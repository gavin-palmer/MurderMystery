using MurderMystery.Data.Providers;

namespace MurderMystery.Data
{
    /// <summary>
    /// Factory for creating and accessing data providers
    /// </summary>
    public static class DataProviderFactory
    {
        private static WeaponProvider _weaponProvider;
        private static RoomProvider _roomProvider;
        private static MotiveProvider _motiveProvider;
        private static ActionProvider _actionProvider;
        private static NameProvider _nameProvider;
        private static ClueProvider _clueProvider;

        /// <summary>
        /// Gets the weapon provider
        /// </summary>
        public static WeaponProvider Weapons
        {
            get
            {
                if (_weaponProvider == null)
                    _weaponProvider = new WeaponProvider();

                return _weaponProvider;
            }
        }

        /// <summary>
        /// Gets the room provider
        /// </summary>
        public static RoomProvider Rooms
        {
            get
            {
                if (_roomProvider == null)
                    _roomProvider = new RoomProvider();

                return _roomProvider;
            }
        }

        /// <summary>
        /// Gets the motive provider
        /// </summary>
        public static MotiveProvider Motives
        {
            get
            {
                if (_motiveProvider == null)
                    _motiveProvider = new MotiveProvider();

                return _motiveProvider;
            }
        }

        /// <summary>
        /// Gets the action provider
        /// </summary>
        public static ActionProvider Actions
        {
            get
            {
                if (_actionProvider == null)
                    _actionProvider = new ActionProvider();

                return _actionProvider;
            }
        }

        /// <summary>
        /// Gets the name provider
        /// </summary>
        public static NameProvider Names
        {
            get
            {
                if (_nameProvider == null)
                    _nameProvider = new NameProvider();

                return _nameProvider;
            }
        }

        /// <summary>
        /// Gets the clue provider
        /// </summary>
        public static ClueProvider Clues
        {
            get
            {
                if (_clueProvider == null)
                    _clueProvider = new ClueProvider();

                return _clueProvider;
            }
        }
    }
}