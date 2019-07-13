package com.tfg.marta.androidplugin2;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Context;
import android.os.Build;
import android.os.CountDownTimer;
import android.os.PowerManager;
import android.support.v4.app.NotificationCompat;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.io.Console;

import static android.content.Context.POWER_SERVICE;

public class PluginClass {

    private CountDownTimer CDT;
    private final String CHANNEL_ID = "0"; //Canal para las notificaciones

    private static Activity myActivity; //Actividad obtenida desde unity

    //Desde Unity se invoca a este metodo para pasarle la actividad principal
    public static void receiveActivityInstance(Activity tempActivity) {
        myActivity = tempActivity;
    }

    //Crea una cuenta atras con la duracion que se recibe como parametro
    public void CreateCDT (int millis) {
        CDT = new CountDownTimer(millis, 1000) {

            //En cada tick (cada segundo), envia a Unity el numero de segundos restantes
            public void onTick(long millisUntilFinished) {
                UnityPlayer.UnitySendMessage("Countdown", "TimeLeft", Long.toString(millisUntilFinished/1000));
                Log.d("CDT", "tick");
            }

            //Cuando termina la cuenta atras, envia un mensaje a Unity y muestra una notificacion
            public void onFinish() {
                UnityPlayer.UnitySendMessage("Countdown", "EndCDT", "FIN");
                Log.d("CDT", "FINISH");
                SendNotification();
            }
        };
    }

    //Comenzar la cuenta atras
    public void StartCDT () {
        CDT.start();
    }

    //Interrumpir la cuenta atras
    public void StopCDT () {
        CDT.cancel();
    }

    //Crea una notificacion indicando que ha finalizado la cuenta atras
    public void SendNotification () {
        Log.d("CDT", "NOTIFC");
        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(myActivity.getApplicationContext(), CHANNEL_ID)
                .setSmallIcon(R.drawable.notif_icon)
                .setContentTitle("Fin del tiempo")
                .setContentText("Es hora de tomarse un descanso")
                .setVibrate(new long[] {100, 500})
                .setPriority(NotificationCompat.PRIORITY_HIGH);

        NotificationManager mNotificationManager = (NotificationManager) myActivity.getSystemService(Context.NOTIFICATION_SERVICE);
        mNotificationManager.notify(0, mBuilder.build());
    }

    //Crear un canal para la notificacion (solo necesario a partir de la API 26)
    private void CreateNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            Log.d("Unity", "Crear canales");
            CharSequence name = "canalCDT";
            String description = "canalCDT";
            int importance = NotificationManager.IMPORTANCE_HIGH;
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, importance);
            channel.setDescription(description);
            NotificationManager notificationManager = myActivity.getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }

    //Comprobar si la pantalla esta o no encendida
    public boolean IsScreenOn () {
        PowerManager powerManager = (PowerManager) myActivity.getSystemService(POWER_SERVICE);
        boolean isScreenOn = powerManager.isScreenOn();
        Log.d("unity","Pantalla: "+Boolean.toString(isScreenOn));
        return isScreenOn;
    }

}
