
package ai.nreal.videoplayer;

import android.content.Context;
import android.net.Uri;
import android.util.Log;
import android.view.Surface;
import android.widget.Toast;
import com.google.android.exoplayer2.C;
import com.google.android.exoplayer2.MediaItem;
import com.google.android.exoplayer2.SimpleExoPlayer;
import com.google.android.exoplayer2.MediaItem.DrmConfiguration;
import com.google.android.exoplayer2.MediaItem.PlaybackProperties;
import com.google.android.exoplayer2.SimpleExoPlayer.Builder;
import com.google.android.exoplayer2.drm.FrameworkMediaDrm;
import com.google.android.exoplayer2.util.Assertions;
import com.google.android.exoplayer2.util.Util;

public class ExoMediaPlayer implements IVideoPlayer {
    private static final String TAG = "ExoMediaPlayer";
    public SimpleExoPlayer player;
    private IVideoPlayerEventProxy eventProxy;
    private Context mContext;
    private static final String ACTION_VIEW = "com.google.android.exoplayer.surfacedemo.action.VIEW";
    private static final String EXTENSION_EXTRA = "extension";
    private static final String DRM_SCHEME_EXTRA = "widevine";
    public static final String MIME_TYPE_DASH = "application/dash+xml";
    public static final String MIME_TYPE_HLS = "application/x-mpegURL";
    public static final String MIME_TYPE_SS = "application/vnd.ms-sstr+xml";
    public static final String MIME_TYPE_VIDEO_MP4 = "video/mp4";

    public ExoMediaPlayer() {
        Log.i("ExoMediaPlayer", "ExoMediaPlayer: ");
    }

    public void init(Context context, IVideoPlayerEventProxy event) {
        Log.i("ExoMediaPlayer", "init: ");
        this.player = (new Builder(context)).build();
        this.eventProxy = event;
        this.mContext = context;
    }

    public void setSurface(Surface mSurface) {
        this.player.setVideoSurface(mSurface);
    }

    public void load(String path, boolean isDrm) {
        Log.i("ExoMediaPlayer", "load: " + path);
        MediaItem item = null;
        if (isDrm) {
            item = this.getDRMItem(path);
        } else {
            item = this.getNormalItem(path);
        }

        this.player.setMediaItem(item);
        this.player.prepare();
        this.player.play();
        this.player.setRepeatMode(2);
        this.eventProxy.OnEvent(10001);
    }

    private MediaItem getDRMItem(String path) {
        MediaItem item = (new com.google.android.exoplayer2.MediaItem.Builder()).setUri(Uri.parse(path)).setMediaMetadata((new com.google.android.exoplayer2.MediaMetadata.Builder()).setTitle("Widevine DASH cenc: Tears").build()).setMimeType("application/dash+xml").setDrmUuid(C.WIDEVINE_UUID).setDrmLicenseUri("https://proxy.uat.widevine.com/proxy?provider=widevine_test").build();
        this.checkMediaItem(item);
        return item;
    }

    private MediaItem getNormalItem(String path) {
        return (new com.google.android.exoplayer2.MediaItem.Builder()).setUri(Uri.parse(path)).build();
    }

    private void checkMediaItem(MediaItem mediaItem) {
        if (!Util.checkCleartextTrafficPermitted(new MediaItem[]{mediaItem})) {
            Log.i("ExoMediaPlayer", "checkMediaItem: ");
        }

        DrmConfiguration drmConfiguration = ((PlaybackProperties)Assertions.checkNotNull(mediaItem.playbackProperties)).drmConfiguration;
        if (drmConfiguration != null) {
            if (Util.SDK_INT < 18) {
                Log.i("ExoMediaPlayer", "checkMediaItem: error_drm_unsupported_before_api_18");
            } else if (!FrameworkMediaDrm.isCryptoSchemeSupported(drmConfiguration.uuid)) {
                Log.i("ExoMediaPlayer", "checkMediaItem: error_drm_unsupported_scheme");
            }
        }

    }

    private void showToast(int messageId) {
        this.showToast(this.mContext.getString(messageId));
    }

    private void showToast(String message) {
        Toast.makeText(this.mContext, message, 1).show();
    }

    public void play() {
        Log.i("ExoMediaPlayer", "play: ");
        this.player.play();
    }

    public void pause() {
        Log.i("ExoMediaPlayer", "pause: ");
        this.player.pause();
    }

    public void release() {
        Log.i("ExoMediaPlayer", "release: ");
        this.player.release();
    }
}
