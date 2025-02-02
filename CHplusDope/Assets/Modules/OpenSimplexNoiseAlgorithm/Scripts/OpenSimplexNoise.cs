﻿/**
 * K.jpg's OpenSimplex 2, faster variant
 *
 * - 2D is standard simplex implemented using a lookup table.
 * - 3D is "Re-oriented 4-point BCC noise" which constructs a
 *   congruent BCC lattice in a much different way than usual.
 * - 4D constructs the lattice as a union of five copies of its
 *   reciprocal. It successively finds the closest point on each.
 *
 * Multiple versions of each function are provided. See the
 * documentation above each, for more info.
 */

using System;
using System.Runtime.CompilerServices;

namespace OpenSimplexNoiseAlgorithm
{
    public class OpenSimplexNoise
    {
        private const int PSIZE = 2048;
        private const int PMASK = 2047;

        private short[] perm;
        private Grad2[] permGrad2;
        private Grad3[] permGrad3;
        private Grad4[] permGrad4;

        public OpenSimplexNoise(long seed)
        {
            perm = new short[PSIZE];
            permGrad2 = new Grad2[PSIZE];
            permGrad3 = new Grad3[PSIZE];
            permGrad4 = new Grad4[PSIZE];
            short[] source = new short[PSIZE];
            for (short i = 0; i < PSIZE; i++)
                source[i] = i;
            for (int i = PSIZE - 1; i >= 0; i--)
            {
                seed = seed * 6364136223846793005L + 1442695040888963407L;
                int r = (int)((seed + 31) % (i + 1));
                if (r < 0)
                    r += (i + 1);
                perm[i] = source[r];
                permGrad2[i] = GRADIENTS_2D[perm[i]];
                permGrad3[i] = GRADIENTS_3D[perm[i]];
                permGrad4[i] = GRADIENTS_4D[perm[i]];
                source[r] = source[i];
            }
        }

        /*
         * Noise Evaluators
         */

        /**
         * 2D Simplex noise, standard lattice orientation.
         */
        public float Evaluate(float x, float y)
        {

            // Get points for A2* lattice
            float s = 0.366025403784439f * (x + y);
            float xs = x + s, ys = y + s;

            return noise2_Base(xs, ys);
        }

        /**
         * 2D Simplex noise, with Y pointing down the main diagonal.
         * Might be better for a 2D sandbox style game, where Y is vertical.
         * Probably slightly less optimal for heightmaps or continent maps.
         */
        public float EvaluateVertical(float x, float y)
        {

            // Skew transform and rotation baked into one.
            float xx = x * 0.7071067811865476f;
            float yy = y * 1.224744871380249f;

            return noise2_Base(yy + xx, yy - xx);
        }

        /**
         * 2D Simplex noise base.
         * Lookup table implementation inspired by DigitalShadow.
         */
        private float noise2_Base(float xs, float ys)
        {
            float value = 0;

            // Get base points and offsets
            int xsb = fastFloor(xs), ysb = fastFloor(ys);
            float xsi = xs - xsb, ysi = ys - ysb;

            // Index to point list
            int index = (int)((ysi - xsi) / 2 + 1);

            float ssi = (xsi + ysi) * -0.211324865405187f;
            float xi = xsi + ssi, yi = ysi + ssi;

            // Point contributions
            for (int i = 0; i < 3; i++)
            {
                LatticePoint2D c = LOOKUP_2D[index + i];

                float dx = xi + c.dx, dy = yi + c.dy;
                float attn = 0.5f - dx * dx - dy * dy;
                if (attn <= 0) continue;

                int pxm = (xsb + c.xsv) & PMASK, pym = (ysb + c.ysv) & PMASK;
                Grad2 grad = permGrad2[perm[pxm] ^ pym];
                float extrapolation = grad.dx * dx + grad.dy * dy;

                attn *= attn;
                value += attn * attn * extrapolation;
            }

            return value;
        }

        /**
         * 3D Re-oriented 4-point BCC noise, classic orientation.
         * Proper substitute for 3D Simplex in light of Forbidden Formulae.
         * Use noise3_XYBeforeZ or noise3_XZBeforeY instead, wherever appropriate.
         */
        public float Evaluate(float x, float y, float z)
        {

            // Re-orient the cubic lattices via rotation, to produce the expected look on cardinal planar slices.
            // If texturing objects that don't tend to have cardinal plane faces, you could even remove this.
            // Orthonormal rotation. Not a skew transform.
            float r = (2.0f / 3.0f) * (x + y + z);
            float xr = r - x, yr = r - y, zr = r - z;

            // Evaluate both lattices to form a BCC lattice.
            return noise3_BCC(xr, yr, zr);
        }

        /**
         * 3D Re-oriented 4-point BCC noise, with better visual isotropy in (X, Y).
         * Recommended for 3D terrain and time-varied animations.
         * The Z coordinate should always be the "different" coordinate in your use case.
         * If Y is vertical in world coordinates, call noise3_XYBeforeZ(x, z, Y) or use noise3_XZBeforeY.
         * If Z is vertical in world coordinates, call noise3_XYBeforeZ(x, y, Z).
         * For a time varied animation, call noise3_XYBeforeZ(x, y, T).
         */
        public float EvaluateXYBeforeZ(float x, float y, float z)
        {

            // Re-orient the cubic lattices without skewing, to make X and Y triangular like 2D.
            // Orthonormal rotation. Not a skew transform.
            float xy = x + y;
            float s2 = xy * -0.211324865405187f;
            float zz = z * 0.577350269189626f;
            float xr = x + s2 - zz, yr = y + s2 - zz;
            float zr = xy * 0.577350269189626f + zz;

            // Evaluate both lattices to form a BCC lattice.
            return noise3_BCC(xr, yr, zr);
        }

        /**
         * 3D Re-oriented 4-point BCC noise, with better visual isotropy in (X, Z).
         * Recommended for 3D terrain and time-varied animations.
         * The Y coordinate should always be the "different" coordinate in your use case.
         * If Y is vertical in world coordinates, call noise3_XZBeforeY(x, Y, z).
         * If Z is vertical in world coordinates, call noise3_XZBeforeY(x, Z, y) or use noise3_XYBeforeZ.
         * For a time varied animation, call noise3_XZBeforeY(x, T, y) or use noise3_XYBeforeZ.
         */
        public float EvaluateXZBeforeY(float x, float y, float z)
        {

            // Re-orient the cubic lattices without skewing, to make X and Z triangular like 2D.
            // Orthonormal rotation. Not a skew transform.
            float xz = x + z;
            float s2 = xz * -0.211324865405187f;
            float yy = y * 0.577350269189626f;
            float xr = x + s2 - yy; float zr = z + s2 - yy;
            float yr = xz * 0.577350269189626f + yy;

            // Evaluate both lattices to form a BCC lattice.
            return noise3_BCC(xr, yr, zr);
        }

        /**
         * Generate overlapping cubic lattices for 3D Re-oriented BCC noise.
         * Lookup table implementation inspired by DigitalShadow.
         * It was actually faster to narrow down the points in the loop itself,
         * than to build up the index with enough info to isolate 4 points.
         */
        private float noise3_BCC(float xr, float yr, float zr)
        {

            // Get base and offsets inside cube of first lattice.
            int xrb = fastFloor(xr), yrb = fastFloor(yr), zrb = fastFloor(zr);
            float xri = xr - xrb, yri = yr - yrb, zri = zr - zrb;

            // Identify which octant of the cube we're in. This determines which cell
            // in the other cubic lattice we're in, and also narrows down one point on each.
            int xht = (int)(xri + 0.5), yht = (int)(yri + 0.5), zht = (int)(zri + 0.5);
            int index = (xht << 0) | (yht << 1) | (zht << 2);

            // Point contributions
            float value = 0;
            LatticePoint3D c = LOOKUP_3D[index];
            while (c != null)
            {
                float dxr = xri + c.dxr, dyr = yri + c.dyr, dzr = zri + c.dzr;
                float attn = 0.5f - dxr * dxr - dyr * dyr - dzr * dzr;
                if (attn < 0)
                {
                    c = c.NextOnFailure;
                }
                else
                {
                    int pxm = (xrb + c.xrv) & PMASK, pym = (yrb + c.yrv) & PMASK, pzm = (zrb + c.zrv) & PMASK;
                    Grad3 grad = permGrad3[perm[perm[pxm] ^ pym] ^ pzm];
                    float extrapolation = grad.dx * dxr + grad.dy * dyr + grad.dz * dzr;

                    attn *= attn;
                    value += attn * attn * extrapolation;
                    c = c.NextOnSuccess;
                }
            }
            return value;
        }

        /**
         * 4D OpenSimplex2F noise, classic lattice orientation.
         */
        public float Evaluate(float x, float y, float z, float w)
        {

            // Get points for A4 lattice
            float s = -0.138196601125011f * (x + y + z + w);
            float xs = x + s, ys = y + s, zs = z + s, ws = w + s;

            return noise4_Base(xs, ys, zs, ws);
        }

        /**
         * 4D OpenSimplex2F noise, with XY and ZW forming orthogonal triangular-based planes.
         * Recommended for 3D terrain, where X and Y (or Z and W) are horizontal.
         * Recommended for noise(x, y, sin(time), cos(time)) trick.
         */
        public float EvaluateXYBeforeZW(float x, float y, float z, float w)
        {

            float s2 = (x + y) * -0.178275657951399372f + (z + w) * 0.215623393288842828f;
            float t2 = (z + w) * -0.403949762580207112f + (x + y) * -0.375199083010075342f;
            float xs = x + s2, ys = y + s2, zs = z + t2, ws = w + t2;

            return noise4_Base(xs, ys, zs, ws);
        }

        /**
         * 4D OpenSimplex2F noise, with XZ and YW forming orthogonal triangular-based planes.
         * Recommended for 3D terrain, where X and Z (or Y and W) are horizontal.
         */
        public float EvaluateXZBeforeYW(float x, float y, float z, float w)
        {

            float s2 = (x + z) * -0.178275657951399372f + (y + w) * 0.215623393288842828f;
            float t2 = (y + w) * -0.403949762580207112f + (x + z) * -0.375199083010075342f;
            float xs = x + s2, ys = y + t2, zs = z + s2, ws = w + t2;

            return noise4_Base(xs, ys, zs, ws);
        }

        /**
         * 4D OpenSimplex2F noise, with XYZ oriented like noise3_Classic,
         * and W for an extra degree of freedom. W repeats eventually.
         * Recommended for time-varied animations which texture a 3D object (W=time)
         */
        public float EvaluateXYZBeforeW(float x, float y, float z, float w)
        {

            float xyz = x + y + z;
            float ww = w * 0.2236067977499788f;
            float s2 = xyz * -0.16666666666666666f + ww;
            float xs = x + s2, ys = y + s2, zs = z + s2, ws = -0.5f * xyz + ww;

            return noise4_Base(xs, ys, zs, ws);
        }

        /**
         * 4D OpenSimplex2F noise base.
         * Current implementation not fully optimized by lookup tables.
         * But still comes out slightly ahead of Gustavson's Simplex in tests.
         */
        private float noise4_Base(float xs, float ys, float zs, float ws)
        {
            float value = 0;

            // Get base points and offsets
            int xsb = fastFloor(xs), ysb = fastFloor(ys), zsb = fastFloor(zs), wsb = fastFloor(ws);
            float xsi = xs - xsb, ysi = ys - ysb, zsi = zs - zsb, wsi = ws - wsb;

            // If we're in the lower half, flip so we can repeat the code for the upper half. We'll flip back later.
            float siSum = xsi + ysi + zsi + wsi;
            float ssi = siSum * 0.309016994374947f; // Prep for vertex contributions.
            bool inLowerHalf = (siSum < 2);
            if (inLowerHalf)
            {
                xsi = 1 - xsi; ysi = 1 - ysi; zsi = 1 - zsi; wsi = 1 - wsi;
                siSum = 4 - siSum;
            }

            // Consider opposing vertex pairs of the octahedron formed by the central cross-section of the stretched tesseract
            float aabb = xsi + ysi - zsi - wsi, abab = xsi - ysi + zsi - wsi, abba = xsi - ysi - zsi + wsi;
            float aabbScore = Math.Abs(aabb), ababScore = Math.Abs(abab), abbaScore = Math.Abs(abba);

            // Find the closest point on the stretched tesseract as if it were the upper half
            int vertexIndex, via, vib;
            float asi, bsi;
            if (aabbScore > ababScore && aabbScore > abbaScore)
            {
                if (aabb > 0)
                {
                    asi = zsi; bsi = wsi; vertexIndex = 0b0011; via = 0b0111; vib = 0b1011;
                }
                else
                {
                    asi = xsi; bsi = ysi; vertexIndex = 0b1100; via = 0b1101; vib = 0b1110;
                }
            }
            else if (ababScore > abbaScore)
            {
                if (abab > 0)
                {
                    asi = ysi; bsi = wsi; vertexIndex = 0b0101; via = 0b0111; vib = 0b1101;
                }
                else
                {
                    asi = xsi; bsi = zsi; vertexIndex = 0b1010; via = 0b1011; vib = 0b1110;
                }
            }
            else
            {
                if (abba > 0)
                {
                    asi = ysi; bsi = zsi; vertexIndex = 0b1001; via = 0b1011; vib = 0b1101;
                }
                else
                {
                    asi = xsi; bsi = wsi; vertexIndex = 0b0110; via = 0b0111; vib = 0b1110;
                }
            }
            if (bsi > asi)
            {
                via = vib;
                float temp = bsi;
                bsi = asi;
                asi = temp;
            }
            if (siSum + asi > 3)
            {
                vertexIndex = via;
                if (siSum + bsi > 4)
                {
                    vertexIndex = 0b1111;
                }
            }

            // Now flip back if we're actually in the lower half.
            if (inLowerHalf)
            {
                xsi = 1 - xsi; ysi = 1 - ysi; zsi = 1 - zsi; wsi = 1 - wsi;
                vertexIndex ^= 0b1111;
            }

            // Five points to add, total, from five copies of the A4 lattice.
            for (int i = 0; i < 5; i++)
            {

                // Update xsb/etc. and add the lattice point's contribution.
                LatticePoint4D c = VERTICES_4D[vertexIndex];
                xsb += c.xsv; ysb += c.ysv; zsb += c.zsv; wsb += c.wsv;
                float xi = xsi + ssi, yi = ysi + ssi, zi = zsi + ssi, wi = wsi + ssi;
                float dx = xi + c.dx, dy = yi + c.dy, dz = zi + c.dz, dw = wi + c.dw;
                float attn = 0.5f - dx * dx - dy * dy - dz * dz - dw * dw;
                if (attn > 0)
                {
                    int pxm = xsb & PMASK, pym = ysb & PMASK, pzm = zsb & PMASK, pwm = wsb & PMASK;
                    Grad4 grad = permGrad4[perm[perm[perm[pxm] ^ pym] ^ pzm] ^ pwm];
                    float ramped = grad.dx * dx + grad.dy * dy + grad.dz * dz + grad.dw * dw;

                    attn *= attn;
                    value += attn * attn * ramped;
                }

                // Maybe this helps the compiler/JVM/LLVM/etc. know we can end the loop here. Maybe not.
                if (i == 4) break;

                // Update the relative skewed coordinates to reference the vertex we just added.
                // Rather, reference its counterpart on the lattice copy that is shifted down by
                // the vector <-0.2, -0.2, -0.2, -0.2>
                xsi += c.xsi; ysi += c.ysi; zsi += c.zsi; wsi += c.wsi;
                ssi += c.ssiDelta;

                // Next point is the closest vertex on the 4-simplex whose base vertex is the aforementioned vertex.
                float score0 = 1.0f + ssi * (-1.0f / 0.309016994374947f); // Seems slightly faster than 1.0-xsi-ysi-zsi-wsi
                vertexIndex = 0b0000;
                if (xsi >= ysi && xsi >= zsi && xsi >= wsi && xsi >= score0)
                {
                    vertexIndex = 0b0001;
                }
                else if (ysi > xsi && ysi >= zsi && ysi >= wsi && ysi >= score0)
                {
                    vertexIndex = 0b0010;
                }
                else if (zsi > xsi && zsi > ysi && zsi >= wsi && zsi >= score0)
                {
                    vertexIndex = 0b0100;
                }
                else if (wsi > xsi && wsi > ysi && wsi > zsi && wsi >= score0)
                {
                    vertexIndex = 0b1000;
                }
            }

            return value;
        }

        /*
         * Utility
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int fastFloor(float x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        /*
         * Lookup Tables & Gradients
         */

        private static LatticePoint2D[] LOOKUP_2D;
        private static LatticePoint3D[] LOOKUP_3D;
        private static LatticePoint4D[] VERTICES_4D;

        private const float N2 = 0.01001634121365712f;
        private const float N3 = 0.030485933181293584f;
        private const float N4 = 0.009202377986303158f;
        private static Grad2[] GRADIENTS_2D;
        private static Grad3[] GRADIENTS_3D;
        private static Grad4[] GRADIENTS_4D;

        static OpenSimplexNoise()
        {
            LOOKUP_2D = new LatticePoint2D[4];
            LOOKUP_3D = new LatticePoint3D[8];
            VERTICES_4D = new LatticePoint4D[16];
            
            LOOKUP_2D[0] = new LatticePoint2D(1, 0);
            LOOKUP_2D[1] = new LatticePoint2D(0, 0);
            LOOKUP_2D[2] = new LatticePoint2D(1, 1);
            LOOKUP_2D[3] = new LatticePoint2D(0, 1);

            for (int i = 0; i < 8; i++)
            {
                int i1, j1, k1, i2, j2, k2;
                i1 = (i >> 0) & 1; j1 = (i >> 1) & 1; k1 = (i >> 2) & 1;
                i2 = i1 ^ 1; j2 = j1 ^ 1; k2 = k1 ^ 1;

                // The two points within this octant, one from each of the two cubic half-lattices.
                LatticePoint3D c0 = new LatticePoint3D(i1, j1, k1, 0);
                LatticePoint3D c1 = new LatticePoint3D(i1 + i2, j1 + j2, k1 + k2, 1);

                // Each single step away on the first half-lattice.
                LatticePoint3D c2 = new LatticePoint3D(i1 ^ 1, j1, k1, 0);
                LatticePoint3D c3 = new LatticePoint3D(i1, j1 ^ 1, k1, 0);
                LatticePoint3D c4 = new LatticePoint3D(i1, j1, k1 ^ 1, 0);

                // Each single step away on the second half-lattice.
                LatticePoint3D c5 = new LatticePoint3D(i1 + (i2 ^ 1), j1 + j2, k1 + k2, 1);
                LatticePoint3D c6 = new LatticePoint3D(i1 + i2, j1 + (j2 ^ 1), k1 + k2, 1);
                LatticePoint3D c7 = new LatticePoint3D(i1 + i2, j1 + j2, k1 + (k2 ^ 1), 1);

                // First two are guaranteed.
                c0.NextOnFailure = c0.NextOnSuccess = c1;
                c1.NextOnFailure = c1.NextOnSuccess = c2;

                // Once we find one on the first half-lattice, the rest are out.
                // In addition, knowing c2 rules out c5.
                c2.NextOnFailure = c3; c2.NextOnSuccess = c6;
                c3.NextOnFailure = c4; c3.NextOnSuccess = c5;
                c4.NextOnFailure = c4.NextOnSuccess = c5;

                // Once we find one on the second half-lattice, the rest are out.
                c5.NextOnFailure = c6; c5.NextOnSuccess = null;
                c6.NextOnFailure = c7; c6.NextOnSuccess = null;
                c7.NextOnFailure = c7.NextOnSuccess = null;

                LOOKUP_3D[i] = c0;
            }

            for (int i = 0; i < 16; i++)
            {
                VERTICES_4D[i] = new LatticePoint4D((i >> 0) & 1, (i >> 1) & 1, (i >> 2) & 1, (i >> 3) & 1);
            }

            GRADIENTS_2D = new Grad2[PSIZE];
            Grad2[] grad2 = {
                new Grad2( 0.130526192220052f,  0.99144486137381f),
                new Grad2( 0.38268343236509f,   0.923879532511287f),
                new Grad2( 0.608761429008721f,  0.793353340291235f),
                new Grad2( 0.793353340291235f,  0.608761429008721f),
                new Grad2( 0.923879532511287f,  0.38268343236509f),
                new Grad2( 0.99144486137381f,   0.130526192220051f),
                new Grad2( 0.99144486137381f,  -0.130526192220051f),
                new Grad2( 0.923879532511287f, -0.38268343236509f),
                new Grad2( 0.793353340291235f, -0.60876142900872f),
                new Grad2( 0.608761429008721f, -0.793353340291235f),
                new Grad2( 0.38268343236509f,  -0.923879532511287f),
                new Grad2( 0.130526192220052f, -0.99144486137381f),
                new Grad2(-0.130526192220052f, -0.99144486137381f),
                new Grad2(-0.38268343236509f,  -0.923879532511287f),
                new Grad2(-0.608761429008721f, -0.793353340291235f),
                new Grad2(-0.793353340291235f, -0.608761429008721f),
                new Grad2(-0.923879532511287f, -0.38268343236509f),
                new Grad2(-0.99144486137381f,  -0.130526192220052f),
                new Grad2(-0.99144486137381f,   0.130526192220051f),
                new Grad2(-0.923879532511287f,  0.38268343236509f),
                new Grad2(-0.793353340291235f,  0.608761429008721f),
                new Grad2(-0.608761429008721f,  0.793353340291235f),
                new Grad2(-0.38268343236509f,   0.923879532511287f),
                new Grad2(-0.130526192220052f,  0.99144486137381f)
            };
            for (int i = 0; i < grad2.Length; i++)
            {
                grad2[i].dx /= N2; grad2[i].dy /= N2;
            }
            for (int i = 0; i < PSIZE; i++)
            {
                GRADIENTS_2D[i] = grad2[i % grad2.Length];
            }

            GRADIENTS_3D = new Grad3[PSIZE];
            Grad3[] grad3 = {
                new Grad3(-2.22474487139f,      -2.22474487139f,      -1.0f),
                new Grad3(-2.22474487139f,      -2.22474487139f,       1.0f),
                new Grad3(-3.0862664687972017f, -1.1721513422464978f,  0.0f),
                new Grad3(-1.1721513422464978f, -3.0862664687972017f,  0.0f),
                new Grad3(-2.22474487139f,      -1.0f,                -2.22474487139f),
                new Grad3(-2.22474487139f,       1.0f,                -2.22474487139f),
                new Grad3(-1.1721513422464978f,  0.0f,                -3.0862664687972017f),
                new Grad3(-3.0862664687972017f,  0.0f,                -1.1721513422464978f),
                new Grad3(-2.22474487139f,      -1.0f,                 2.22474487139f),
                new Grad3(-2.22474487139f,       1.0f,                 2.22474487139f),
                new Grad3(-3.0862664687972017f,  0.0f,                 1.1721513422464978f),
                new Grad3(-1.1721513422464978f,  0.0f,                 3.0862664687972017f),
                new Grad3(-2.22474487139f,       2.22474487139f,      -1.0f),
                new Grad3(-2.22474487139f,       2.22474487139f,       1.0f),
                new Grad3(-1.1721513422464978f,  3.0862664687972017f,  0.0f),
                new Grad3(-3.0862664687972017f,  1.1721513422464978f,  0.0f),
                new Grad3(-1.0f,                -2.22474487139f,      -2.22474487139f),
                new Grad3( 1.0f,                -2.22474487139f,      -2.22474487139f),
                new Grad3( 0.0f,                -3.0862664687972017f, -1.1721513422464978f),
                new Grad3( 0.0f,                -1.1721513422464978f, -3.0862664687972017f),
                new Grad3(-1.0f,                -2.22474487139f,       2.22474487139f),
                new Grad3( 1.0f,                -2.22474487139f,       2.22474487139f),
                new Grad3( 0.0f,                -1.1721513422464978f,  3.0862664687972017f),
                new Grad3( 0.0f,                -3.0862664687972017f,  1.1721513422464978f),
                new Grad3(-1.0f,                 2.22474487139f,      -2.22474487139f),
                new Grad3( 1.0f,                 2.22474487139f,      -2.22474487139f),
                new Grad3( 0.0f,                 1.1721513422464978f, -3.0862664687972017f),
                new Grad3( 0.0f,                 3.0862664687972017f, -1.1721513422464978f),
                new Grad3(-1.0f,                 2.22474487139f,       2.22474487139f),
                new Grad3( 1.0f,                 2.22474487139f,       2.22474487139f),
                new Grad3( 0.0f,                 3.0862664687972017f,  1.1721513422464978f),
                new Grad3( 0.0f,                 1.1721513422464978f,  3.0862664687972017f),
                new Grad3( 2.22474487139f,      -2.22474487139f,      -1.0f),
                new Grad3( 2.22474487139f,      -2.22474487139f,       1.0f),
                new Grad3( 1.1721513422464978f, -3.0862664687972017f,  0.0f),
                new Grad3( 3.0862664687972017f, -1.1721513422464978f,  0.0f),
                new Grad3( 2.22474487139f,      -1.0f,                -2.22474487139f),
                new Grad3( 2.22474487139f,       1.0f,                -2.22474487139f),
                new Grad3( 3.0862664687972017f,  0.0f,                -1.1721513422464978f),
                new Grad3( 1.1721513422464978f,  0.0f,                -3.0862664687972017f),
                new Grad3( 2.22474487139f,      -1.0f,                 2.22474487139f),
                new Grad3( 2.22474487139f,       1.0f,                 2.22474487139f),
                new Grad3( 1.1721513422464978f,  0.0f,                 3.0862664687972017f),
                new Grad3( 3.0862664687972017f,  0.0f,                 1.1721513422464978f),
                new Grad3( 2.22474487139f,       2.22474487139f,      -1.0f),
                new Grad3( 2.22474487139f,       2.22474487139f,       1.0f),
                new Grad3( 3.0862664687972017f,  1.1721513422464978f,  0.0f),
                new Grad3( 1.1721513422464978f,  3.0862664687972017f,  0.0f)
            };
            for (int i = 0; i < grad3.Length; i++)
            {
                grad3[i].dx /= N3; grad3[i].dy /= N3; grad3[i].dz /= N3;
            }
            for (int i = 0; i < PSIZE; i++)
            {
                GRADIENTS_3D[i] = grad3[i % grad3.Length];
            }

            GRADIENTS_4D = new Grad4[PSIZE];
            Grad4[] grad4 = {
                new Grad4(-0.753341017856078,    -0.37968289875261624,  -0.37968289875261624,  -0.37968289875261624),
                new Grad4(-0.7821684431180708,   -0.4321472685365301,   -0.4321472685365301,    0.12128480194602098),
                new Grad4(-0.7821684431180708,   -0.4321472685365301,    0.12128480194602098,  -0.4321472685365301),
                new Grad4(-0.7821684431180708,    0.12128480194602098,  -0.4321472685365301,   -0.4321472685365301),
                new Grad4(-0.8586508742123365,   -0.508629699630796,     0.044802370851755174,  0.044802370851755174),
                new Grad4(-0.8586508742123365,    0.044802370851755174, -0.508629699630796,     0.044802370851755174),
                new Grad4(-0.8586508742123365,    0.044802370851755174,  0.044802370851755174, -0.508629699630796),
                new Grad4(-0.9982828964265062,   -0.03381941603233842,  -0.03381941603233842,  -0.03381941603233842),
                new Grad4(-0.37968289875261624,  -0.753341017856078,    -0.37968289875261624,  -0.37968289875261624),
                new Grad4(-0.4321472685365301,   -0.7821684431180708,   -0.4321472685365301,    0.12128480194602098),
                new Grad4(-0.4321472685365301,   -0.7821684431180708,    0.12128480194602098,  -0.4321472685365301),
                new Grad4( 0.12128480194602098,  -0.7821684431180708,   -0.4321472685365301,   -0.4321472685365301),
                new Grad4(-0.508629699630796,    -0.8586508742123365,    0.044802370851755174,  0.044802370851755174),
                new Grad4( 0.044802370851755174, -0.8586508742123365,   -0.508629699630796,     0.044802370851755174),
                new Grad4( 0.044802370851755174, -0.8586508742123365,    0.044802370851755174, -0.508629699630796),
                new Grad4(-0.03381941603233842,  -0.9982828964265062,   -0.03381941603233842,  -0.03381941603233842),
                new Grad4(-0.37968289875261624,  -0.37968289875261624,  -0.753341017856078,    -0.37968289875261624),
                new Grad4(-0.4321472685365301,   -0.4321472685365301,   -0.7821684431180708,    0.12128480194602098),
                new Grad4(-0.4321472685365301,    0.12128480194602098,  -0.7821684431180708,   -0.4321472685365301),
                new Grad4( 0.12128480194602098,  -0.4321472685365301,   -0.7821684431180708,   -0.4321472685365301),
                new Grad4(-0.508629699630796,     0.044802370851755174, -0.8586508742123365,    0.044802370851755174),
                new Grad4( 0.044802370851755174, -0.508629699630796,    -0.8586508742123365,    0.044802370851755174),
                new Grad4( 0.044802370851755174,  0.044802370851755174, -0.8586508742123365,   -0.508629699630796),
                new Grad4(-0.03381941603233842,  -0.03381941603233842,  -0.9982828964265062,   -0.03381941603233842),
                new Grad4(-0.37968289875261624,  -0.37968289875261624,  -0.37968289875261624,  -0.753341017856078),
                new Grad4(-0.4321472685365301,   -0.4321472685365301,    0.12128480194602098,  -0.7821684431180708),
                new Grad4(-0.4321472685365301,    0.12128480194602098,  -0.4321472685365301,   -0.7821684431180708),
                new Grad4( 0.12128480194602098,  -0.4321472685365301,   -0.4321472685365301,   -0.7821684431180708),
                new Grad4(-0.508629699630796,     0.044802370851755174,  0.044802370851755174, -0.8586508742123365),
                new Grad4( 0.044802370851755174, -0.508629699630796,     0.044802370851755174, -0.8586508742123365),
                new Grad4( 0.044802370851755174,  0.044802370851755174, -0.508629699630796,    -0.8586508742123365),
                new Grad4(-0.03381941603233842,  -0.03381941603233842,  -0.03381941603233842,  -0.9982828964265062),
                new Grad4(-0.6740059517812944,   -0.3239847771997537,   -0.3239847771997537,    0.5794684678643381),
                new Grad4(-0.7504883828755602,   -0.4004672082940195,    0.15296486218853164,   0.5029860367700724),
                new Grad4(-0.7504883828755602,    0.15296486218853164,  -0.4004672082940195,    0.5029860367700724),
                new Grad4(-0.8828161875373585,    0.08164729285680945,   0.08164729285680945,   0.4553054119602712),
                new Grad4(-0.4553054119602712,   -0.08164729285680945,  -0.08164729285680945,   0.8828161875373585),
                new Grad4(-0.5029860367700724,   -0.15296486218853164,   0.4004672082940195,    0.7504883828755602),
                new Grad4(-0.5029860367700724,    0.4004672082940195,   -0.15296486218853164,   0.7504883828755602),
                new Grad4(-0.5794684678643381,    0.3239847771997537,    0.3239847771997537,    0.6740059517812944),
                new Grad4(-0.3239847771997537,   -0.6740059517812944,   -0.3239847771997537,    0.5794684678643381),
                new Grad4(-0.4004672082940195,   -0.7504883828755602,    0.15296486218853164,   0.5029860367700724),
                new Grad4( 0.15296486218853164,  -0.7504883828755602,   -0.4004672082940195,    0.5029860367700724),
                new Grad4( 0.08164729285680945,  -0.8828161875373585,    0.08164729285680945,   0.4553054119602712),
                new Grad4(-0.08164729285680945,  -0.4553054119602712,   -0.08164729285680945,   0.8828161875373585),
                new Grad4(-0.15296486218853164,  -0.5029860367700724,    0.4004672082940195,    0.7504883828755602),
                new Grad4( 0.4004672082940195,   -0.5029860367700724,   -0.15296486218853164,   0.7504883828755602),
                new Grad4( 0.3239847771997537,   -0.5794684678643381,    0.3239847771997537,    0.6740059517812944),
                new Grad4(-0.3239847771997537,   -0.3239847771997537,   -0.6740059517812944,    0.5794684678643381),
                new Grad4(-0.4004672082940195,    0.15296486218853164,  -0.7504883828755602,    0.5029860367700724),
                new Grad4( 0.15296486218853164,  -0.4004672082940195,   -0.7504883828755602,    0.5029860367700724),
                new Grad4( 0.08164729285680945,   0.08164729285680945,  -0.8828161875373585,    0.4553054119602712),
                new Grad4(-0.08164729285680945,  -0.08164729285680945,  -0.4553054119602712,    0.8828161875373585),
                new Grad4(-0.15296486218853164,   0.4004672082940195,   -0.5029860367700724,    0.7504883828755602),
                new Grad4( 0.4004672082940195,   -0.15296486218853164,  -0.5029860367700724,    0.7504883828755602),
                new Grad4( 0.3239847771997537,    0.3239847771997537,   -0.5794684678643381,    0.6740059517812944),
                new Grad4(-0.6740059517812944,   -0.3239847771997537,    0.5794684678643381,   -0.3239847771997537),
                new Grad4(-0.7504883828755602,   -0.4004672082940195,    0.5029860367700724,    0.15296486218853164),
                new Grad4(-0.7504883828755602,    0.15296486218853164,   0.5029860367700724,   -0.4004672082940195),
                new Grad4(-0.8828161875373585,    0.08164729285680945,   0.4553054119602712,    0.08164729285680945),
                new Grad4(-0.4553054119602712,   -0.08164729285680945,   0.8828161875373585,   -0.08164729285680945),
                new Grad4(-0.5029860367700724,   -0.15296486218853164,   0.7504883828755602,    0.4004672082940195),
                new Grad4(-0.5029860367700724,    0.4004672082940195,    0.7504883828755602,   -0.15296486218853164),
                new Grad4(-0.5794684678643381,    0.3239847771997537,    0.6740059517812944,    0.3239847771997537),
                new Grad4(-0.3239847771997537,   -0.6740059517812944,    0.5794684678643381,   -0.3239847771997537),
                new Grad4(-0.4004672082940195,   -0.7504883828755602,    0.5029860367700724,    0.15296486218853164),
                new Grad4( 0.15296486218853164,  -0.7504883828755602,    0.5029860367700724,   -0.4004672082940195),
                new Grad4( 0.08164729285680945,  -0.8828161875373585,    0.4553054119602712,    0.08164729285680945),
                new Grad4(-0.08164729285680945,  -0.4553054119602712,    0.8828161875373585,   -0.08164729285680945),
                new Grad4(-0.15296486218853164,  -0.5029860367700724,    0.7504883828755602,    0.4004672082940195),
                new Grad4( 0.4004672082940195,   -0.5029860367700724,    0.7504883828755602,   -0.15296486218853164),
                new Grad4( 0.3239847771997537,   -0.5794684678643381,    0.6740059517812944,    0.3239847771997537),
                new Grad4(-0.3239847771997537,   -0.3239847771997537,    0.5794684678643381,   -0.6740059517812944),
                new Grad4(-0.4004672082940195,    0.15296486218853164,   0.5029860367700724,   -0.7504883828755602),
                new Grad4( 0.15296486218853164,  -0.4004672082940195,    0.5029860367700724,   -0.7504883828755602),
                new Grad4( 0.08164729285680945,   0.08164729285680945,   0.4553054119602712,   -0.8828161875373585),
                new Grad4(-0.08164729285680945,  -0.08164729285680945,   0.8828161875373585,   -0.4553054119602712),
                new Grad4(-0.15296486218853164,   0.4004672082940195,    0.7504883828755602,   -0.5029860367700724),
                new Grad4( 0.4004672082940195,   -0.15296486218853164,   0.7504883828755602,   -0.5029860367700724),
                new Grad4( 0.3239847771997537,    0.3239847771997537,    0.6740059517812944,   -0.5794684678643381),
                new Grad4(-0.6740059517812944,    0.5794684678643381,   -0.3239847771997537,   -0.3239847771997537),
                new Grad4(-0.7504883828755602,    0.5029860367700724,   -0.4004672082940195,    0.15296486218853164),
                new Grad4(-0.7504883828755602,    0.5029860367700724,    0.15296486218853164,  -0.4004672082940195),
                new Grad4(-0.8828161875373585,    0.4553054119602712,    0.08164729285680945,   0.08164729285680945),
                new Grad4(-0.4553054119602712,    0.8828161875373585,   -0.08164729285680945,  -0.08164729285680945),
                new Grad4(-0.5029860367700724,    0.7504883828755602,   -0.15296486218853164,   0.4004672082940195),
                new Grad4(-0.5029860367700724,    0.7504883828755602,    0.4004672082940195,   -0.15296486218853164),
                new Grad4(-0.5794684678643381,    0.6740059517812944,    0.3239847771997537,    0.3239847771997537),
                new Grad4(-0.3239847771997537,    0.5794684678643381,   -0.6740059517812944,   -0.3239847771997537),
                new Grad4(-0.4004672082940195,    0.5029860367700724,   -0.7504883828755602,    0.15296486218853164),
                new Grad4( 0.15296486218853164,   0.5029860367700724,   -0.7504883828755602,   -0.4004672082940195),
                new Grad4( 0.08164729285680945,   0.4553054119602712,   -0.8828161875373585,    0.08164729285680945),
                new Grad4(-0.08164729285680945,   0.8828161875373585,   -0.4553054119602712,   -0.08164729285680945),
                new Grad4(-0.15296486218853164,   0.7504883828755602,   -0.5029860367700724,    0.4004672082940195),
                new Grad4( 0.4004672082940195,    0.7504883828755602,   -0.5029860367700724,   -0.15296486218853164),
                new Grad4( 0.3239847771997537,    0.6740059517812944,   -0.5794684678643381,    0.3239847771997537),
                new Grad4(-0.3239847771997537,    0.5794684678643381,   -0.3239847771997537,   -0.6740059517812944),
                new Grad4(-0.4004672082940195,    0.5029860367700724,    0.15296486218853164,  -0.7504883828755602),
                new Grad4( 0.15296486218853164,   0.5029860367700724,   -0.4004672082940195,   -0.7504883828755602),
                new Grad4( 0.08164729285680945,   0.4553054119602712,    0.08164729285680945,  -0.8828161875373585),
                new Grad4(-0.08164729285680945,   0.8828161875373585,   -0.08164729285680945,  -0.4553054119602712),
                new Grad4(-0.15296486218853164,   0.7504883828755602,    0.4004672082940195,   -0.5029860367700724),
                new Grad4( 0.4004672082940195,    0.7504883828755602,   -0.15296486218853164,  -0.5029860367700724),
                new Grad4( 0.3239847771997537,    0.6740059517812944,    0.3239847771997537,   -0.5794684678643381),
                new Grad4( 0.5794684678643381,   -0.6740059517812944,   -0.3239847771997537,   -0.3239847771997537),
                new Grad4( 0.5029860367700724,   -0.7504883828755602,   -0.4004672082940195,    0.15296486218853164),
                new Grad4( 0.5029860367700724,   -0.7504883828755602,    0.15296486218853164,  -0.4004672082940195),
                new Grad4( 0.4553054119602712,   -0.8828161875373585,    0.08164729285680945,   0.08164729285680945),
                new Grad4( 0.8828161875373585,   -0.4553054119602712,   -0.08164729285680945,  -0.08164729285680945),
                new Grad4( 0.7504883828755602,   -0.5029860367700724,   -0.15296486218853164,   0.4004672082940195),
                new Grad4( 0.7504883828755602,   -0.5029860367700724,    0.4004672082940195,   -0.15296486218853164),
                new Grad4( 0.6740059517812944,   -0.5794684678643381,    0.3239847771997537,    0.3239847771997537),
                new Grad4( 0.5794684678643381,   -0.3239847771997537,   -0.6740059517812944,   -0.3239847771997537),
                new Grad4( 0.5029860367700724,   -0.4004672082940195,   -0.7504883828755602,    0.15296486218853164),
                new Grad4( 0.5029860367700724,    0.15296486218853164,  -0.7504883828755602,   -0.4004672082940195),
                new Grad4( 0.4553054119602712,    0.08164729285680945,  -0.8828161875373585,    0.08164729285680945),
                new Grad4( 0.8828161875373585,   -0.08164729285680945,  -0.4553054119602712,   -0.08164729285680945),
                new Grad4( 0.7504883828755602,   -0.15296486218853164,  -0.5029860367700724,    0.4004672082940195),
                new Grad4( 0.7504883828755602,    0.4004672082940195,   -0.5029860367700724,   -0.15296486218853164),
                new Grad4( 0.6740059517812944,    0.3239847771997537,   -0.5794684678643381,    0.3239847771997537),
                new Grad4( 0.5794684678643381,   -0.3239847771997537,   -0.3239847771997537,   -0.6740059517812944),
                new Grad4( 0.5029860367700724,   -0.4004672082940195,    0.15296486218853164,  -0.7504883828755602),
                new Grad4( 0.5029860367700724,    0.15296486218853164,  -0.4004672082940195,   -0.7504883828755602),
                new Grad4( 0.4553054119602712,    0.08164729285680945,   0.08164729285680945,  -0.8828161875373585),
                new Grad4( 0.8828161875373585,   -0.08164729285680945,  -0.08164729285680945,  -0.4553054119602712),
                new Grad4( 0.7504883828755602,   -0.15296486218853164,   0.4004672082940195,   -0.5029860367700724),
                new Grad4( 0.7504883828755602,    0.4004672082940195,   -0.15296486218853164,  -0.5029860367700724),
                new Grad4( 0.6740059517812944,    0.3239847771997537,    0.3239847771997537,   -0.5794684678643381),
                new Grad4( 0.03381941603233842,   0.03381941603233842,   0.03381941603233842,   0.9982828964265062),
                new Grad4(-0.044802370851755174, -0.044802370851755174,  0.508629699630796,     0.8586508742123365),
                new Grad4(-0.044802370851755174,  0.508629699630796,    -0.044802370851755174,  0.8586508742123365),
                new Grad4(-0.12128480194602098,   0.4321472685365301,    0.4321472685365301,    0.7821684431180708),
                new Grad4( 0.508629699630796,    -0.044802370851755174, -0.044802370851755174,  0.8586508742123365),
                new Grad4( 0.4321472685365301,   -0.12128480194602098,   0.4321472685365301,    0.7821684431180708),
                new Grad4( 0.4321472685365301,    0.4321472685365301,   -0.12128480194602098,   0.7821684431180708),
                new Grad4( 0.37968289875261624,   0.37968289875261624,   0.37968289875261624,   0.753341017856078),
                new Grad4( 0.03381941603233842,   0.03381941603233842,   0.9982828964265062,    0.03381941603233842),
                new Grad4(-0.044802370851755174,  0.044802370851755174,  0.8586508742123365,    0.508629699630796),
                new Grad4(-0.044802370851755174,  0.508629699630796,     0.8586508742123365,   -0.044802370851755174),
                new Grad4(-0.12128480194602098,   0.4321472685365301,    0.7821684431180708,    0.4321472685365301),
                new Grad4( 0.508629699630796,    -0.044802370851755174,  0.8586508742123365,   -0.044802370851755174),
                new Grad4( 0.4321472685365301,   -0.12128480194602098,   0.7821684431180708,    0.4321472685365301),
                new Grad4( 0.4321472685365301,    0.4321472685365301,    0.7821684431180708,   -0.12128480194602098),
                new Grad4( 0.37968289875261624,   0.37968289875261624,   0.753341017856078,     0.37968289875261624),
                new Grad4( 0.03381941603233842,   0.9982828964265062,    0.03381941603233842,   0.03381941603233842),
                new Grad4(-0.044802370851755174,  0.8586508742123365,   -0.044802370851755174,  0.508629699630796),
                new Grad4(-0.044802370851755174,  0.8586508742123365,    0.508629699630796,    -0.044802370851755174),
                new Grad4(-0.12128480194602098,   0.7821684431180708,    0.4321472685365301,    0.4321472685365301),
                new Grad4( 0.508629699630796,     0.8586508742123365,   -0.044802370851755174, -0.044802370851755174),
                new Grad4( 0.4321472685365301,    0.7821684431180708,   -0.12128480194602098,   0.4321472685365301),
                new Grad4( 0.4321472685365301,    0.7821684431180708,    0.4321472685365301,   -0.12128480194602098),
                new Grad4( 0.37968289875261624,   0.753341017856078,     0.37968289875261624,   0.37968289875261624),
                new Grad4( 0.9982828964265062,    0.03381941603233842,   0.03381941603233842,   0.03381941603233842),
                new Grad4( 0.8586508742123365,   -0.044802370851755174, -0.044802370851755174,  0.508629699630796),
                new Grad4( 0.8586508742123365,   -0.044802370851755174,  0.508629699630796,    -0.044802370851755174),
                new Grad4( 0.7821684431180708,   -0.12128480194602098,   0.4321472685365301,    0.4321472685365301),
                new Grad4( 0.8586508742123365,    0.508629699630796,    -0.044802370851755174, -0.044802370851755174),
                new Grad4( 0.7821684431180708,    0.4321472685365301,   -0.12128480194602098,   0.4321472685365301),
                new Grad4( 0.7821684431180708,    0.4321472685365301,    0.4321472685365301,   -0.12128480194602098),
                new Grad4( 0.753341017856078,     0.37968289875261624,   0.37968289875261624,   0.37968289875261624)
            };
            for (int i = 0; i < grad4.Length; i++)
            {
                grad4[i].dx /= N4; grad4[i].dy /= N4; grad4[i].dz /= N4; grad4[i].dw /= N4;
            }
            for (int i = 0; i < PSIZE; i++)
            {
                GRADIENTS_4D[i] = grad4[i % grad4.Length];
            }
        }

        private struct LatticePoint2D
        {
            public int xsv, ysv;
            public float dx, dy;
            public LatticePoint2D(int xsv, int ysv)
            {
                this.xsv = xsv; this.ysv = ysv;
                float ssv = (xsv + ysv) * -0.211324865405187f;
                this.dx = -xsv - ssv;
                this.dy = -ysv - ssv;
            }
        }

        private class LatticePoint3D
        {
            public float dxr, dyr, dzr;
            public int xrv, yrv, zrv;
            public LatticePoint3D NextOnFailure, NextOnSuccess;
            public LatticePoint3D(int xrv, int yrv, int zrv, int lattice)
            {
                this.dxr = -xrv + lattice * 0.5f; this.dyr = -yrv + lattice * 0.5f; this.dzr = -zrv + lattice * 0.5f;
                this.xrv = xrv + lattice * 1024; this.yrv = yrv + lattice * 1024; this.zrv = zrv + lattice * 1024;
            }
        }

        private struct LatticePoint4D
        {
            public int xsv, ysv, zsv, wsv;
            public float dx, dy, dz, dw;
            public float xsi, ysi, zsi, wsi;
            public float ssiDelta;
            public LatticePoint4D(int xsv, int ysv, int zsv, int wsv)
            {
                this.xsv = xsv + 409; this.ysv = ysv + 409; this.zsv = zsv + 409; this.wsv = wsv + 409;
                float ssv = (xsv + ysv + zsv + wsv) * 0.309016994374947f;
                this.dx = -xsv - ssv;
                this.dy = -ysv - ssv;
                this.dz = -zsv - ssv;
                this.dw = -wsv - ssv;
                this.xsi = xsi = 0.2f - xsv;
                this.ysi = ysi = 0.2f - ysv;
                this.zsi = zsi = 0.2f - zsv;
                this.wsi = wsi = 0.2f - wsv;
                this.ssiDelta = (0.8f - xsv - ysv - zsv - wsv) * 0.309016994374947f;
            }
        }

        private struct Grad2
        {
            public float dx, dy;
            public Grad2(float dx, float dy)
            {
                this.dx = dx; this.dy = dy;
            }
        }

        private struct Grad3
        {
            public float dx, dy, dz;
            public Grad3(float dx, float dy, float dz)
            {
                this.dx = dx; this.dy = dy; this.dz = dz;
            }
        }

        private struct Grad4
        {
            public float dx, dy, dz, dw;
            public Grad4(double dx, double dy, double dz, double dw)
            {
                this.dx = (float) dx; this.dy = (float) dy; this.dz = (float) dz; this.dw = (float) dw;
            }
        }
    }
}
