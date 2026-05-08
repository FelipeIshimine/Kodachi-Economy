using System;
using KodachiGames.UI;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Samples
{
    [Serializable]
    [SelectorName("Economy/Inventory — Grant & Revoke")]
    public class InventoryTest : EconomyTestCase
    {
        public override void Build(VisualElement root)
        {
            root.SetPadding(24);
            root.style.flexDirection = FlexDirection.Column;
            root.Add(SampleUI.H1("Inventory"));
            root.Add(SampleUI.Body("Grant and revoke products. Listen for ownership changes."));

            var inventory = new Inventory();
            var logBox    = SampleUI.LogBox(out var log);

            var car   = SampleDataBuilder.BuildFreeProduct("car.sports",   "Sports Car");
            var skin  = SampleDataBuilder.BuildFreeProduct("skin.gold",    "Gold Skin");
            var track = SampleDataBuilder.BuildFreeProduct("track.desert", "Desert Track");

            var ownedDisplay = SampleUI.StatusLabel("Owned: (none)");
            var owned = 0;

            void RefreshDisplay() =>
                ownedDisplay.text = owned == 0 ? "Owned: (none)" : $"Owned: {owned} item(s)";

            inventory.OnProductGranted += product =>
            {
                owned++;
                log($"✓ Granted: {product.DisplayName}");
                RefreshDisplay();
            };

            inventory.OnProductRevoked += product =>
            {
                owned--;
                log($"✗ Revoked: {product.DisplayName}");
                RefreshDisplay();
            };

            var controls = SampleUI.Row();

            controls.Add(SampleUI.Button("Grant Car", () =>
            {
                if (!inventory.HasProduct(car))
                    inventory.Grant(car);
                else
                    log("(car already owned)");
            }));

            controls.Add(SampleUI.Button("Grant Skin", () =>
            {
                if (!inventory.HasProduct(skin))
                    inventory.Grant(skin);
                else
                    log("(skin already owned)");
            }));

            controls.Add(SampleUI.Button("Grant Track", () =>
            {
                if (!inventory.HasProduct(track))
                    inventory.Grant(track);
                else
                    log("(track already owned)");
            }));

            controls.Add(SampleUI.Button("Revoke Car", () =>
            {
                if (inventory.HasProduct(car))
                    inventory.Revoke(car);
                else
                    log("(car not owned)");
            }));

            controls.Add(SampleUI.Button("Revoke All", () =>
            {
                foreach (var product in new[] { car, skin, track })
                    if (inventory.HasProduct(product))
                        inventory.Revoke(product);
            }));

            root.Add(ownedDisplay);
            root.Add(logBox);
            root.Add(controls);
        }
    }
}
