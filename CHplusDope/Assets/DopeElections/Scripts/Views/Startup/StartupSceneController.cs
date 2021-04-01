using AppManagement;
using BlackMasks;
using Essentials;
using UnityEngine;

namespace DopeElections.Startup
{
    public class StartupSceneController : SceneController, IBlackMaskProvider
    {
        public override NamespacedKey Id => SceneId.Startup;
        
        [SerializeField] private StartupViewsContainer _viewsContainer = null;
        [SerializeField] private BlackMask _blackMask = null;

        public StartupViewsContainer Views => _viewsContainer;
        public BlackMask BlackMask => _blackMask;

        private bool _initialized = false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            InitializeViews();
        }

        protected override void OnLoad()
        {
        }

        public void InitializeViews()
        {
            if (_initialized) return;
            _initialized = true;
            Views.Initialize();
        }
    }
}