
namespace BobboNet.SGB.IMod
{
    public static class SGBPauseMenuOptions
    {
        /// <summary>
        /// Options relating to a button on SGB's pause menu.
        /// </summary>
        [System.Serializable]
        public class MenuButtonOptions
        {
            /// <summary>
            /// Is this button visible?
            /// </summary>
            public bool IsVisible { get; set; } = true;

            /// <summary>
            /// Can the user interact with this button?
            /// </summary>
            public bool IsInteractable { get; set; } = true;
        }

        //
        //  Public Properties
        //

        /// <summary>
        /// The button labeled "Items" by default in the SGB pause menu.
        /// This will dsiplay the SGB player's current inventory when selected.
        /// </summary>
        public static MenuButtonOptions ItemsButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Skills" by default in the SGB pause menu.
        /// This will display the SGB player's current actions / skills when selected.
        /// </summary>
        public static MenuButtonOptions SkillsButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Equipment" by default in the SGB pause menu.
        /// This will display what the SGB player currently has equipped.
        /// </summary>
        public static MenuButtonOptions EquipmentButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Status" by default in the SGB pause menu.
        /// This will display details about the SGB player's stats.
        /// </summary>
        public static MenuButtonOptions StatusButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Save" by default in the SGB pause menu.
        /// This will display a UI allowing the SGB player to save their game.
        /// </summary>
        public static MenuButtonOptions SaveButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Config" by default in the SGB pause menu.
        /// This will display a UI allowing the SGB player to configure their current settings.
        /// </summary>
        public static MenuButtonOptions ConfigButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Close" by default in the SGB pause menu.
        /// This will exit the game.
        /// </summary>
        public static MenuButtonOptions CloseButton { get; private set; } = new MenuButtonOptions();

        /// <summary>
        /// The button labeled "Exit" by default in the SGB pause menu.
        /// </summary>
        /// 
        public static MenuButtonOptions ExitButton { get; private set; } = new MenuButtonOptions();

        //
        //  Constructor
        //

        static SGBPauseMenuOptions()
        {

        }
    }
}