using System;

namespace AppManagement
{
    public class PostSceneLoadAction : SceneLoadParams
    {
        public Action Callback { get; }

        public PostSceneLoadAction(Action callback)
        {
            Callback = callback;
        }
    }
}