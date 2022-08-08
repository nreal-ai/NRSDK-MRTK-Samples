
package ai.nreal.videoplayer;

public interface IVideoPlayerEventProxy {
    int ONPREPARED = 10001;
    int ONFRAMEREADY = 10002;
    int ONCOMPLETED = 10003;
    int ONERROR = 10004;

    void OnEvent(int var1);
}
