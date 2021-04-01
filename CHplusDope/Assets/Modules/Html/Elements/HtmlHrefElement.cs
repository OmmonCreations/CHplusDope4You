using UnityEngine;
using UnityEngine.UI;

namespace Html
{
    public class HtmlHrefElement : HtmlElement
    {
        [SerializeField] private Button _button = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _button.onClick.AddListener(Click);
        }

        private void Click()
        {
            var url = GetAttribute("href");
            if (!string.IsNullOrWhiteSpace(url))
            {
                Application.OpenURL(url);
            }
        }
    }
}