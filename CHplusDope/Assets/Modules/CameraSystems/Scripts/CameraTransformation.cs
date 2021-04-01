using Essentials;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CameraSystems
{
    [System.Serializable]
    public struct CameraTransformation
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public float distance;
        public float fov;

        public CameraTransformation(CameraTransformation template)
        {
            position = template.position;
            eulerAngles = template.eulerAngles;
            distance = template.distance;
            fov = template.fov;
        }

        public static CameraTransformation Lerp(CameraTransformation a, CameraTransformation b, float t)
        {
            var rotAx = MathUtil.Wrap(a.eulerAngles.x, 360);
            var rotBx = MathUtil.Wrap(b.eulerAngles.x, 360);
            if (rotBx - rotAx > 180) rotBx -= 360;
            else if (rotAx - rotBx > 180) rotAx -= 360;

            var rotAy = MathUtil.Wrap(a.eulerAngles.y, 360);
            var rotBy = MathUtil.Wrap(b.eulerAngles.y, 360);
            if (rotBy - rotAy > 180) rotBy -= 360;
            else if (rotAy - rotBy > 180) rotAy -= 360;

            var rotAz = MathUtil.Wrap(a.eulerAngles.z, 360);
            var rotBz = MathUtil.Wrap(b.eulerAngles.z, 360);
            if (rotBz - rotAz > 180) rotBz -= 360;
            else if (rotAz - rotBz > 180) rotAz -= 360;

            var rotX = Mathf.Lerp(rotAx, rotBx, t);
            var rotY = Mathf.Lerp(rotAy, rotBy, t);
            var rotZ = Mathf.Lerp(rotAz, rotBz, t);

            return new CameraTransformation()
            {
                position = Vector3.Lerp(a.position, b.position, t),
                eulerAngles = new Vector3(rotX, rotY, rotZ),
                distance = Mathf.Lerp(a.distance, b.distance, t),
                fov = Mathf.Lerp(a.fov, b.fov, t)
            };
        }

        public CameraTransformation Clone()
        {
            return new CameraTransformation(this);
        }

        public override string ToString()
        {
            return "(position: " + position + ", eulerAngles: " + eulerAngles + ", fov: " + fov + ", distance: " +
                   distance + ")";
        }

        public JToken Serialize()
        {
            return new JObject()
            {
                ["px"] = position.x,
                ["py"] = position.y,
                ["pz"] = position.z,
                ["rx"] = eulerAngles.x,
                ["ry"] = eulerAngles.y,
                ["rz"] = eulerAngles.z,
                ["fov"] = fov,
                ["d"] = distance,
            };
        }

        public static bool TryParse(string s, out CameraTransformation result)
        {
            try
            {
                return TryParse(JObject.Parse(s), out result);
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool TryParse(JToken json, out CameraTransformation result)
        {
            return TryParse(json as JObject, out result);
        }

        public static bool TryParse(JObject json, out CameraTransformation result)
        {
            if (json == null)
            {
                result = default;
                return false;
            }

            var px = json["px"] != null ? (float) json["px"] : 0;
            var py = json["py"] != null ? (float) json["py"] : 0;
            var pz = json["pz"] != null ? (float) json["pz"] : 0;
            var rx = json["rx"] != null ? (float) json["rx"] : 0;
            var ry = json["ry"] != null ? (float) json["ry"] : 0;
            var rz = json["rz"] != null ? (float) json["rz"] : 0;
            var fov = json["fov"] != null ? (float) json["fov"] : 30;
            var distance = json["d"] != null ? (float) json["d"] : 10;

            result = new CameraTransformation
            {
                position = new Vector3(px, py, pz),
                eulerAngles = new Vector3(rx, ry, rz),
                fov = fov,
                distance = distance
            };

            return true;
        }
    }
}