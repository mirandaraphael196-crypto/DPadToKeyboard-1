using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DPadToKeyboard;

/// <summary>
/// Remaps the Bluetooth controller D-pad to keyboard keys:
/// DPadUp -> U, DPadRight -> B, DPadDown -> G, DPadLeft -> A.
/// The original D-pad input is suppressed so it won't move the farmer.
/// </summary>
internal sealed class ModEntry : Mod
{
    private static readonly (SButton Source, SButton Target)[] Mappings =
    {
        (SButton.DPadUp, SButton.U),
        (SButton.DPadRight, SButton.B),
        (SButton.DPadDown, SButton.G),
        (SButton.DPadLeft, SButton.A)
    };

    public override void Entry(IModHelper helper)
    {
        // Suppress immediately when a D-pad direction is pressed.
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        // Re-send the mapped key while the D-pad direction remains held.
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

        this.Monitor.Log("D-pad remapping loaded: Up=U, Right=B, Down=G, Left=A.", LogLevel.Info);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        foreach ((SButton source, SButton target) in Mappings)
        {
            if (e.Button != source)
                continue;

            this.Helper.Input.Suppress(source);
            this.Helper.Input.Press(target);
            return;
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        foreach ((SButton source, SButton target) in Mappings)
        {
            // IsSuppressed remains true while the physical button is held,
            // even though the game itself sees that source button as released.
            if (this.Helper.Input.IsDown(source) || this.Helper.Input.IsSuppressed(source))
            {
                this.Helper.Input.Suppress(source);
                this.Helper.Input.Press(target);
            }
        }
    }
}
