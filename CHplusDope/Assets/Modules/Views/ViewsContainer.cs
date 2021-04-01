using System.Linq;
using BlackMasks;
using Essentials;
using Localizator;
using Localizator.Settings;
using UnityEngine;

namespace Views
{
    public abstract class ViewsContainer : MonoBehaviour
    {
        [SerializeField] private BlackMask _blackMask = null;
        [SerializeField] private LocalizationScope _localizationScope = null;

        private NavigationTree _navigationTree;
        protected View[] Views { get; private set; }
        public ILocalization Localization
        {
            get => _localizationScope.Localization;
            set => _localizationScope.Localization = value;
        }

        public ILocalizationScope LocalizationScope => _localizationScope;

        public BlackMask BlackMask => _blackMask;

        #region Initializers

        public void Initialize()
        {
            OnBeforeInitialize();

            _navigationTree = BuildNavigationTree();
            Views = _navigationTree.Views;

            foreach (var view in Views)
            {
                if (!view) continue;
                view.gameObject.SetActive(true);
                view.Initialize(this);
                view.Opens += () => CloseViews(view);
                view.Opened += () => CloseViews(view);
                view.gameObject.SetActive(false);
            }

            foreach (var branch in _navigationTree.GetAllBranches())
            {
                branch.View.gameObject.SetActive(branch.Configuration.ShowOnAwake);
            }

            if (_localizationScope)
            {
                var localization = FetchLocalization();
                if (localization != null)
                {
                    _localizationScope.Localization = localization;
                }
            }

            var go = gameObject;
            if (!go.activeSelf)
            {
                go.SetActive(true);
                go.SetActive(false);
            }

            OnInitialize();
        }

        #endregion

        #region Unity Control

        protected void OnEnable()
        {
            OnEnabled();
        }

        protected void OnDisable()
        {
            OnDisabled();
        }

        protected void OnDestroy()
        {
            OnDestroyed();
        }

        #endregion

        #region Public API

        public T GetView<T>() where T : View
        {
            return Views.OfType<T>().FirstOrDefault();
        }

        public View GetView(NamespacedKey viewId)
        {
            return Views.FirstOrDefault(v => v.Id == viewId);
        }

        public T GetView<T>(NamespacedKey viewId) where T : IGenericView
        {
            return Views.OfType<T>().FirstOrDefault(v => v.Id == viewId);
        }

        #endregion

        #region Virtual Members

        protected virtual void OnBeforeInitialize()
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }

        protected virtual void OnDestroyed()
        {
        }

        #endregion

        #region Abstract Members

        protected abstract ILocalization FetchLocalization();

        protected abstract NavigationTree BuildNavigationTree();

        #endregion

        public void CloseViews()
        {
            foreach (var other in Views.Where(m => m.IsOpen))
            {
                other.Close();
            }
        }

        public void CloseViews(View except)
        {
            foreach (var other in Views.Where(m => m != except && m.IsOpen))
            {
                var branch = _navigationTree.GetBranch(other);
                if (branch.View == except || branch.Contains(except))
                {
                    continue;
                }

                other.Close();
            }
        }
    }
}