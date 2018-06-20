package com.example.yair144.imagemobile;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.os.Environment;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.v4.app.NotificationCompat;
import android.widget.Toast;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FilenameFilter;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class ImageService extends Service {

    private BroadcastReceiver onWifiConnect;
    private int count;

    public ImageService() {
        count = 0;
    }

    @Nullable

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        final IntentFilter theFilter = new IntentFilter();
        theFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        theFilter.addAction("android.net.wifi.STATE_CHANGE");
        onWifiConnect = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                        if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                            startTransfer();
                        }
                    }
                }
            }
        };
        this.registerReceiver(this.onWifiConnect, theFilter);
    }
    public int onStartCommand(Intent intent, int flag, int startId){
        Toast.makeText(this,"Service starting...", Toast.LENGTH_SHORT).show();
        return START_STICKY;
    }

    public void onDestroy() {
        Toast.makeText(this,"Service ending...", Toast.LENGTH_SHORT).show();
    }


    private void startTransfer() {
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {

                        //here you must put your computer's IP address.
                        InetAddress serverAddr = InetAddress.getByName("10.0.2.2");

                        //create a socket to make the connection with the server
                        Socket socket = new Socket(serverAddr, 8000);
                        try {
                            //sends the message to the server
                            OutputStream output = socket.getOutputStream();
                            sendImages(output);
                           // output.close();
                        } catch (Exception e) {
                        } finally {
                          //  socket.close();
                        }
                    } catch (Exception e) {

                    }


                }
            }).start();
        }
    private  void sendImages(OutputStream output) throws Exception {
        File dcim = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM+"/Camera");
        if (dcim == null) {
            return;
        }

        final NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
        final NotificationManager notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        final int notify_id = 1;
        final File[] pics = dcim.listFiles(new FilenameFilter() {
            public boolean accept(File dir, String name) {
                return (name.endsWith(".jpg") || name.endsWith(".jpeg") || name.endsWith(".png") ||
                        name.endsWith(".bmp") || name.endsWith(".gif"));
            }
        });
        final int len = pics.length;

        new Thread(new Runnable() {
            @Override
            public void run() {
                while (count < len) {
                    builder.setProgress(len, count, false);
                    builder.setContentText("Transfer in progress\n" + count + "/" + len + "Transferred");
                    notificationManager.notify(notify_id, builder.build());
                }
                builder.setProgress(len, count, false);
                builder.setContentText("Transfer completed");
                notificationManager.notify(notify_id, builder.build());
            }
        }).start();

        if (pics != null) {
            for (File pic : pics) {
                try {
                    count++;
                    SendPhoto(output, pic);
                } catch (Exception e) {

                }
            }
        }

    }

    private byte[] getBytesFromBitmap(Bitmap bitmap) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
        return stream.toByteArray();
    }
    private void SendPhoto(OutputStream output, File pic) throws Exception{
        FileInputStream fis = new FileInputStream(pic);
        Bitmap bm = BitmapFactory.decodeStream(fis);
        byte[] imgbyte = getBytesFromBitmap(bm);
        String name = pic.getName();

        output.write(ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(name.length()).array());
        output.flush();
        output.write(name.getBytes());
        output.flush();
        output.write(ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(imgbyte.length).array());
        output.flush();
        output.write(imgbyte);
        output.flush();
    }

}