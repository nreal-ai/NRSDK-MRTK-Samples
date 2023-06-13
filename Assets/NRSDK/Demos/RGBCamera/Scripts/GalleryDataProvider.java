package ai.nreal.android.gallery;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public class GalleryDataProvider {

    private final static String TAG = GalleryDataProvider.class.getSimpleName();

    private Context mContext;
    private ContentResolver mContentResolver;

    public GalleryDataProvider(Context context) {
        mContext = context;
        mContentResolver = mContext.getContentResolver();
    }
    
    //@RequiresApi(api = Build.VERSION_CODES.Q)
    public Uri insertImage(final Bitmap bitmap, final String displayName,
                           final String folderName) {
        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, displayName);
        values.put(MediaStore.Images.Media.MIME_TYPE, "image/png");
        long currentTime = System.currentTimeMillis() / 1000;

        values.put(MediaStore.Images.ImageColumns.DATE_ADDED, currentTime);
        values.put(MediaStore.Images.ImageColumns.DATE_MODIFIED, currentTime);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            values.put(MediaStore.Images.ImageColumns.DATE_TAKEN, currentTime);
            values.put(MediaStore.MediaColumns.RELATIVE_PATH,
                    Environment.DIRECTORY_PICTURES + File.separator + folderName);
        }
        Uri imageUri = mContentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);

        try (OutputStream outputStream = mContentResolver.openOutputStream(imageUri)) {
            boolean success = bitmap.compress(Bitmap.CompressFormat.PNG, 100, outputStream);
            if (!success) {
                Log.e(TAG, "insertImageAsPNG: bitmap compressed to output stream failed");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        return imageUri;
    }

    //@RequiresApi(api = Build.VERSION_CODES.Q)
    public Uri insertImage(final InputStream inputStream, final String displayName,
                           final String folderName, final String mimeType) {
        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, displayName);
        values.put(MediaStore.Images.Media.MIME_TYPE, mimeType);

        long currentTime = System.currentTimeMillis() / 1000;

        values.put(MediaStore.Images.ImageColumns.DATE_ADDED, currentTime);
        values.put(MediaStore.Images.ImageColumns.DATE_MODIFIED, currentTime);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            values.put(MediaStore.Images.ImageColumns.DATE_TAKEN, currentTime);
            values.put(MediaStore.MediaColumns.RELATIVE_PATH,
                    Environment.DIRECTORY_PICTURES + File.separator + folderName);
        }
        Uri imageUri = mContentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
        if (imageUri == null) {
            return null;
        }

        try (OutputStream outputStream = mContentResolver.openOutputStream(imageUri)) {
            writeStreamToOutput(inputStream, outputStream);
        } catch (IOException e) {
            e.printStackTrace();
        }

        return imageUri;
    }

    //@RequiresApi(api = Build.VERSION_CODES.Q)
    public Uri insertImage(final String imagePath, final String displayName, final String folderName) {
        final BitmapFactory.Options options = new BitmapFactory.Options();
        options.inJustDecodeBounds = true;

        BitmapFactory.decodeFile(imagePath, options);

        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, displayName);
        values.put(MediaStore.Images.Media.MIME_TYPE, options.outMimeType);
        long currentTime = System.currentTimeMillis() / 1000;

        values.put(MediaStore.Images.ImageColumns.DATE_ADDED, currentTime);
        values.put(MediaStore.Images.ImageColumns.DATE_MODIFIED, currentTime);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            values.put(MediaStore.Images.ImageColumns.DATE_TAKEN, currentTime);
            values.put(MediaStore.MediaColumns.RELATIVE_PATH,
                    Environment.DIRECTORY_PICTURES + File.separator + folderName);
        }
        Uri imageUri = mContentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);

        try {
            File file = new File(imagePath);
            FileInputStream inputStream = new FileInputStream(file);
            OutputStream outputStream = mContentResolver.openOutputStream(imageUri);
            writeStreamToOutput(inputStream, outputStream);
        } catch (IOException e) {
            e.printStackTrace();
        }

        return imageUri;
    }

    /**
     * @param inputStream 原始文件流
     * @param displayName 存入的文件名 需要后缀(.mp4)
     * @param folderName  存入的文件夹
     * @param mimeType    类型 ："video/mp4"
     * @return 返回的uri路径
     */
    public Uri insertVideo(final InputStream inputStream, final String displayName,
                           final String folderName, final String mimeType) {
        ContentValues values = new ContentValues();
        values.put(MediaStore.Video.Media.DISPLAY_NAME, displayName);
        values.put(MediaStore.Video.Media.MIME_TYPE, mimeType);
        values.put(MediaStore.MediaColumns.RELATIVE_PATH,
                Environment.DIRECTORY_MOVIES + File.separator + folderName);
        Uri videoUri = mContentResolver.insert(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, values);
        if (videoUri == null) {
            return null;
        }

        try (OutputStream outputStream = mContentResolver.openOutputStream(videoUri)) {
            writeStreamToOutput(inputStream, outputStream);
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return videoUri;
    }

    /**
     * @param originFilePath 原始文件路径
     * @param displayName    存入的文件名 需要后缀(.mp4)
     * @param folderName     存入的文件夹
     * @return 返回的uri路径
     */
    public Uri insertVideo(final String originFilePath, final String displayName,
                           final String folderName) {
        Uri uri = null;
        try {
            FileInputStream inputStream = new FileInputStream(new File(originFilePath));
            uri = insertVideo(inputStream, displayName, folderName, "video/mp4");
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }

        return uri;
    }

    // endregion

    private void writeStreamToOutput(InputStream inputStream, OutputStream outputStream) {
        try {
            byte[] buffer = new byte[4 * 1024]; // or other buffer size
            int read;

            while ((read = inputStream.read(buffer)) != -1) {
                outputStream.write(buffer, 0, read);
            }

            outputStream.flush();
        } catch (FileNotFoundException e) {
            Log.i(TAG, e.toString());
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
