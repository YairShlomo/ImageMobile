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
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

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
                            //startTransferImages();
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




}