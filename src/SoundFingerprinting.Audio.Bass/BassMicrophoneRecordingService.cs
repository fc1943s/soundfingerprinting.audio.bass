namespace SoundFingerprinting.Audio.Bass
{
    using SoundFingerprinting.Infrastructure;

    public class BassMicrophoneRecordingService : IMicrophoneRecordingService
    {
        private readonly IBassServiceProxy proxy;
        private readonly IBassStreamFactory streamFactory;
        private readonly IBassResampler bassResampler;

        public BassMicrophoneRecordingService() : this(DependencyResolver.Current.Get<IBassServiceProxy>(), DependencyResolver.Current.Get<IBassStreamFactory>(), DependencyResolver.Current.Get<IBassResampler>())
        {
            // no op
        }

        internal BassMicrophoneRecordingService(IBassServiceProxy proxy, IBassStreamFactory streamFactory, IBassResampler bassResampler)
        {
            this.proxy = proxy;
            this.streamFactory = streamFactory;
            this.bassResampler = bassResampler;
        }

        public float[] ReadMonoSamplesFromMicrophone(int sampleRate, int secondsToRecord)
        {
            if (!IsRecordingSupported())
            {
                throw new BassException("No recording device could be found un running machine");
            }

            int stream = streamFactory.CreateStreamFromMicrophone(sampleRate);
            return bassResampler.Resample(stream, sampleRate, secondsToRecord, 0, mixerStream => new ContinuousStreamSamplesProvider(new BassSamplesProvider(proxy, mixerStream)));
        }

        private bool IsRecordingSupported()
        {
            return proxy.GetRecordingDevice() != -1;
        }
    }
}