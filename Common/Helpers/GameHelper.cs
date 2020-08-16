﻿using UnityEngine;

namespace Common.Helpers
{
    public static class GameHelper
    { 
        public static void SetProgressColor(Color color)
        {
            HandReticle.main.progressText.color = color;
            HandReticle.main.progressImage.color = color;
        }

        public static void SetInteractColor(Color color, bool isSetSecondary = true)
        {
            HandReticle.main.interactPrimaryText.color = color;

            if (isSetSecondary)
                HandReticle.main.interactSecondaryText.color = color;
        }      

        public static int GetSlotIndex(Vehicle vehicle, TechType techType)
        {
            InventoryItem inventoryItem = null;

            for (int i = 0; i < vehicle.GetSlotCount(); i++)
            {
                try
                {
                    inventoryItem = vehicle.GetSlotItem(i);
                }
                catch
                {
                    continue;
                }

                if (inventoryItem != null && inventoryItem.item.GetTechType() == techType)
                {
                    return vehicle.GetType() == typeof(Exosuit) ? i - 2 : i;
                }
            }

            return -1;
        }
    }
}
