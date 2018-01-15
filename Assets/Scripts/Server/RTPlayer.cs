using System;

namespace Assets.Scripts.Server
{
    public class RTPlayer
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public int PeerId { get; set; }
        public bool IsOnline { get; set; }

        public RTPlayer(string displayName, string id, int peerId)
        {
            DisplayName = displayName;
            Id = id;
            PeerId = peerId;
        }
    }
}
