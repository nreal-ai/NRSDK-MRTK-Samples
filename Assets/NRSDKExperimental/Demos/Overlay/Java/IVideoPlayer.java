
package ai.nreal.videoplayer;

import android.content.Context;
import android.view.Surface;

public interface IVideoPlayer {
    void init(Context var1, IVideoPlayerEventProxy var2);

    void setSurface(Surface var1);

    void load(String path, boolean isDrm);

    void play();

    void pause();

    void release();
}
