#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software,
// and you are welcome to redistribute it under certain conditions; See
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using System;
using MineFort.Entities;
using MineFort.model.entities;

public class ContextMenuAction
{
    public Action<ContextMenuAction, GameCharacter> Action;
    public string Parameter;

    public bool RequireCharacterSelected { get; set; }

    public string LocalizationKey { get; set; }

    public void OnClick(MouseController mouseController)
    {
        if (Action != null)
        {
            if (RequireCharacterSelected)
            {
                if (mouseController.IsCharacterSelected())
                {
                    ISelectable actualSelection = mouseController.mySelection.GetSelectedStuff();
                    Action(this, actualSelection as GameCharacter);
                }
            }
            else
            {
                Action(this, null);
            }
        }
    }
}
