using System;
using Scriprs.Service.Windows;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Scriprs.View
{
    public interface ICustomButton
    {
        string Text { get; set; }
        Action Function { get; set; }
    }

    public class CustomButton : ICustomButton
    {
        public string Text { get; set; }
        public Action Function { get; set; }
        public CustomButton( string text, Action function)
        {
            Text = text;
            Function = function;
        }
    }

    public interface IWindowWithParamsPayload : IWindowPayload
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public CustomButton[] Buttons { get; set; }
    }

    public class WindowWithParamsPayload : IWindowWithParamsPayload
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public CustomButton[] Buttons { get; set; }

        public WindowWithParamsPayload(string title, string body, params CustomButton[] buttons)
        {
            Buttons = buttons;
            Body = body;
            Title = title;
        }
    }

    public class WindowWithParams: BaseWindow
    {
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public TextMeshProUGUI body;
        [SerializeField] public GameObject buttonContainer;
        [SerializeField] public Button buttonPrefub;
        
        protected override void Initialize()
        {
            Id = WindowId.GameActionsWindow;
            if (Payload is WindowWithParamsPayload payload) {
                title.text = payload.Title;
                body.text = payload.Body;

                for (var i = 0; i < payload.Buttons.Length; i++)
                {
                    if (i > 4)
                    {
                        Debug.Log("Too many elements, this UI max 5 button");
                        return;
                    }
                    Button button = Instantiate(buttonPrefub, buttonContainer.transform);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = payload.Buttons[i].Text;
                    var function = payload.Buttons[i].Function;
                    button.onClick.AddListener(() => function?.Invoke());
                }
            }
            else
            {
                Debug.LogWarning("WindowWithParams payload was not a WindowWithParamsPayload");
            }
        }
    }
}