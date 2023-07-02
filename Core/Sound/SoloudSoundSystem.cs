namespace UGXP.Core {
    public class SoloudSoundSystem : ISoundSystem {
        private IntPtr _device;

        public SoloudSoundSystem() {
        }

        public void Init() {
            _device = Soloud.Soloud_create();
            Soloud.Soloud_init(_device);
        }

        public void Deinit() {
            if (_device != IntPtr.Zero) {
                Soloud.Soloud_stopAll(_device);
                Soloud.Soloud_deinit(_device);
                _device = IntPtr.Zero;
            }
        }

        public IntPtr CreateStream(string filename, bool looping) {
            IntPtr id = Soloud.WavStream_create();
            Soloud.WavStream_load(id, filename);
            Soloud.WavStream_setLooping(id, looping);
            if (id == IntPtr.Zero) {
                throw new Exception("Stream file not loaded: " + filename);
            }
            return id;
        }

        public IntPtr LoadSound(string filename, bool looping) {
            IntPtr id = Soloud.Wav_create();
            Soloud.Wav_load(id, filename);
            Soloud.Wav_setLooping(id, looping);
            if (id == IntPtr.Zero) {
                throw new Exception("Sound file not loaded: " + filename);
            }
            return id;
        }

        public void Step() {
            //empty
        }

        public uint PlaySound(IntPtr id, uint channelId, bool paused) {
            if (id == IntPtr.Zero) return 0;
            return Soloud.Soloud_playEx(_device, id, 1.0f, 0.0f, paused, 0);
        }

        public uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan) {
            if (id == IntPtr.Zero) return 0;
            return Soloud.Soloud_playEx(_device, id, volume, pan, paused, 0);
        }


        public float GetChannelFrequency(uint channelId) {
            return Soloud.Soloud_getSamplerate(_device, channelId);
        }

        public void SetChannelFrequency(uint channelId, float frequency) {
            Soloud.Soloud_setSamplerate(_device, channelId, frequency);
        }

        public float GetChannelPan(uint channelId) {
            return Soloud.Soloud_getPan(_device, channelId);
        }

        public void SetChannelPan(uint channelId, float pan) {
            Soloud.Soloud_setPan(_device, channelId, pan);
        }

        public bool GetChannelPaused(uint channelId) {
            return Soloud.Soloud_getPause(_device, channelId);
        }

        public void SetChannelPaused(uint channelId, bool pause) {
            Soloud.Soloud_setPause(_device, channelId, pause);
        }

        public bool ChannelIsPlaying(uint channelId) {
            return (Soloud.Soloud_isValidVoiceHandle(_device, channelId) && !Soloud.Soloud_getPause(_device, channelId));
        }

        public void StopChannel(uint channelId) {
            Soloud.Soloud_stop(_device, channelId);
        }

        public float GetChannelVolume(uint channelId) {
            return Soloud.Soloud_getVolume(_device, channelId);
        }

        public void SetChannelVolume(uint channelId, float volume) {
            Soloud.Soloud_setVolume(_device, channelId, volume);
        }

    }
}
