namespace UGXP.Core {
    public interface ISoundSystem {
        public void Init();
        public void Deinit();
        public IntPtr CreateStream(string filename, bool looping);
        public IntPtr LoadSound(string filename, bool looping);
        public void Step();
        public uint PlaySound(IntPtr id, uint channelId, bool paused);
        public uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan);

        public float GetChannelFrequency(uint channelId);
        public void SetChannelFrequency(uint channelId, float frequency);
        public float GetChannelPan(uint channelId);
        public void SetChannelPan(uint channelId, float pan);
        public float GetChannelVolume(uint channelId);
        public void SetChannelVolume(uint channelId, float volume);
        public bool GetChannelPaused(uint channelId);
        public void SetChannelPaused(uint channelId, bool pause);
        public bool ChannelIsPlaying(uint channelId);
        public void StopChannel(uint channelId);
    }
}