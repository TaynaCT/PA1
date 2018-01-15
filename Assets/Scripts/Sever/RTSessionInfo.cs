using System.Collections.Generic;
using GameSparks.Api.Messages;

namespace Assets.Scripts.Server
{
    public class RTSessionInfo {

	    public string HostUrl { get; set; }
        public string AccessToken { get; set; }
        public int PortId { get; set; }
        public string MatchId { get; set; }
        public List<RTPlayer> RtPlayerList { get; set; }

        public RTSessionInfo(MatchFoundMessage matchFoundMessage)
        {
            PortId = (int)matchFoundMessage.Port;
            HostUrl = matchFoundMessage.Host;
            AccessToken = matchFoundMessage.AccessToken;
            MatchId = matchFoundMessage.MatchId;

            RtPlayerList = new List<RTPlayer>();

            foreach (MatchFoundMessage._Participant participant in matchFoundMessage.Participants)
            {
                RtPlayerList.Add(new RTPlayer(
                    participant.DisplayName, 
                    participant.Id, 
                    (int)participant.PeerId));
            }
        }
    }
}
