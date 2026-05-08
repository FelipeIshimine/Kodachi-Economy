using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Samples
{
    internal static class SampleUI
    {
        internal static Label H1(string text)
        {
            var lbl = new Label(text);
            lbl.style.fontSize                = 22;
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            lbl.style.marginBottom            = 8;
            return lbl;
        }

        internal static Label Body(string text)
        {
            var lbl = new Label(text);
            lbl.style.fontSize     = 11;
            lbl.style.marginBottom = 8;
            return lbl;
        }

        internal static Button Button(string text, Action onClick)
        {
            var btn = new Button(onClick) { text = text };
            btn.style.marginRight  = 8;
            btn.style.marginBottom = 8;
            return btn;
        }

        internal static VisualElement Row()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap      = Wrap.Wrap;
            return row;
        }

        internal static Label StatusLabel(string text)
        {
            var lbl = new Label(text);
            lbl.style.fontSize     = 12;
            lbl.style.marginTop    = 8;
            lbl.style.marginBottom = 8;
            lbl.style.color        = new Color(0.8f, 0.8f, 0.8f);
            return lbl;
        }

        // Returns the scroll view and an Action<string> to append log lines.
        internal static ScrollView LogBox(out Action<string> log)
        {
            var box = new ScrollView();
            box.style.height       = 130;
            box.style.marginTop    = 8;
            box.style.marginBottom = 8;
            ApplyCardBorder(box);

            log = msg =>
            {
                var line = new Label(msg);
                line.style.fontSize   = 11;
                line.style.marginLeft = 6;
                line.style.marginTop  = 2;
                box.Add(line);
                box.ScrollTo(line);
            };

            return box;
        }

        private static void ApplyCardBorder(VisualElement el)
        {
            var grey = new Color(0.4f, 0.4f, 0.4f);
            el.style.borderTopWidth    = el.style.borderBottomWidth =
            el.style.borderLeftWidth   = el.style.borderRightWidth  = 1;
            el.style.borderTopColor    = el.style.borderBottomColor =
            el.style.borderLeftColor   = el.style.borderRightColor  = grey;
        }
    }
}
