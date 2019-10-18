using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace TrueSync {

    /**
     *  @brief Truesync's {@link ICommunicator} implementation based on PUN. 
     **/
    public class PhotonTrueSyncCommunicator : ICommunicator, IOnEventCallback {

        private LoadBalancingPeer loadBalancingPeer;

        private static Action<byte, object, int> lastEventCallback;

        /**
         *  @brief Instantiates a new PhotonTrueSyncCommunicator based on a Photon's LoadbalancingPeer. 
         *  
         *  @param loadBalancingPeer Instance of a Photon's LoadbalancingPeer.
         **/
        internal PhotonTrueSyncCommunicator(LoadBalancingPeer loadBalancingPeer) {
            this.loadBalancingPeer = loadBalancingPeer;
        }

        public int RoundTripTime() {
            return loadBalancingPeer.RoundTripTime;
        }

        public void OnEvent(EventData eventData)
        {
            if (lastEventCallback != null)
            {
                lastEventCallback(eventData.Code, eventData.CustomData, eventData.Sender);
            }
        }
        public void OpRaiseEvent(byte eventCode, object message, bool reliable, int[] toPlayers) {
            if (loadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Connected) {
                return;
            }

            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.TargetActors = toPlayers;

            loadBalancingPeer.OpRaiseEvent(eventCode, message, eventOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }

        public void AddEventListener(OnEventReceived onEventReceived) {
            
            lastEventCallback = delegate (byte eventCode, object content, int senderId) { onEventReceived(eventCode, content); };
            PhotonNetwork.AddCallbackTarget(this);
        }

    }

}
