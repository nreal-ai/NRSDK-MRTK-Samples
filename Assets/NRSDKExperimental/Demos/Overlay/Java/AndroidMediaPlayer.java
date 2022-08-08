//
// Source code recreated from a .class file by IntelliJ IDEA
// (powered by FernFlower decompiler)
//

package ai.nreal.videoplayer;

import android.content.Context;
import android.content.res.AssetFileDescriptor;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.media.MediaPlayer.OnErrorListener;
import android.media.MediaPlayer.OnPreparedListener;
import android.net.Uri;
import android.util.Log;
import android.view.Surface;
import java.io.IOException;

public class AndroidMediaPlayer implements IVideoPlayer {
    private static final String TAG = "AndroidMediaPlayer";
    private MediaPlayer mMediaPlayer = new MediaPlayer();
    private IVideoPlayerEventProxy mVideoPlayerEvent;
    private boolean mIsReady = false;
    private Context mContext;
    private Surface mSurface;

    public AndroidMediaPlayer() {
    }

    public void init(Context context, IVideoPlayerEventProxy event) {
        this.mVideoPlayerEvent = event;
        this.mContext = context;
    }

    public void setSurface(Surface surface) {
        this.mSurface = surface;
        this.mMediaPlayer.setSurface(this.mSurface);
    }

    public void load(String path, boolean isDrm) {
        if (isDrm)
        {
            Log.i("AndroidMediaPlayer", "not support drm content, you can develop your own drm player here : " + path);
            return;
        }
        try {
            Log.i("AndroidMediaPlayer", "load: " + path);
            this.mMediaPlayer.setDataSource(this.mContext, Uri.parse(path));
            this.mMediaPlayer.prepareAsync();
        } catch (IOException var3) {
            var3.printStackTrace();
        }

        final IVideoPlayerEventProxy event = this.mVideoPlayerEvent;
        this.mMediaPlayer.setOnPreparedListener(new OnPreparedListener() {
            public void onPrepared(MediaPlayer mp) {
                AndroidMediaPlayer.this.mIsReady = true;
                event.OnEvent(10001);
                Log.i("AndroidMediaPlayer", "onPrepared: ");
                mp.start();
            }
        });
        this.mMediaPlayer.setOnCompletionListener(new OnCompletionListener() {
            public void onCompletion(MediaPlayer mediaPlayer) {
                event.OnEvent(10003);
                Log.i("AndroidMediaPlayer", "onCompletion: ");
            }
        });
        this.mMediaPlayer.setOnErrorListener(new OnErrorListener() {
            public boolean onError(MediaPlayer mediaPlayer, int i, int i1) {
                event.OnEvent(10004);
                Log.i("AndroidMediaPlayer", "onError: ");
                return false;
            }
        });
    }

    public void loadAsset(AssetFileDescriptor file) {
        try {
            this.mMediaPlayer.setDataSource(file);
            this.mMediaPlayer.prepareAsync();
        } catch (IOException var3) {
            var3.printStackTrace();
        }

        final IVideoPlayerEventProxy event = this.mVideoPlayerEvent;
        this.mMediaPlayer.setOnPreparedListener(new OnPreparedListener() {
            public void onPrepared(MediaPlayer mp) {
                AndroidMediaPlayer.this.mIsReady = true;
                event.OnEvent(10001);
                Log.i("AndroidMediaPlayer", "onPrepared: ");
                mp.start();
            }
        });
        this.mMediaPlayer.setOnCompletionListener(new OnCompletionListener() {
            public void onCompletion(MediaPlayer mediaPlayer) {
                event.OnEvent(10003);
                Log.i("AndroidMediaPlayer", "onCompletion: ");
            }
        });
        this.mMediaPlayer.setOnErrorListener(new OnErrorListener() {
            public boolean onError(MediaPlayer mediaPlayer, int i, int i1) {
                event.OnEvent(10004);
                Log.i("AndroidMediaPlayer", "onError: ");
                return false;
            }
        });
    }

    public void play() {
        if (!this.mMediaPlayer.isPlaying() && this.mIsReady) {
            this.mMediaPlayer.start();
        }

    }

    public void pause() {
        if (this.mIsReady) {
            this.mMediaPlayer.pause();
        }

    }

    public void release() {
        if (this.mIsReady) {
            this.mMediaPlayer.stop();
            this.mMediaPlayer.release();
        }

    }
}
