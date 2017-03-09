package md5372bcbfcb56d8e073f752db8da61fded;


public class GameMap
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.maps.OnMapReadyCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMapReady:(Lcom/google/android/gms/maps/GoogleMap;)V:GetOnMapReady_Lcom_google_android_gms_maps_GoogleMap_Handler:Android.Gms.Maps.IOnMapReadyCallbackInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"";
		mono.android.Runtime.register ("CaptureTheCampus.GameMap, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GameMap.class, __md_methods);
	}


	public GameMap () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GameMap.class)
			mono.android.TypeManager.Activate ("CaptureTheCampus.GameMap, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public GameMap (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == GameMap.class)
			mono.android.TypeManager.Activate ("CaptureTheCampus.GameMap, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onMapReady (com.google.android.gms.maps.GoogleMap p0)
	{
		n_onMapReady (p0);
	}

	private native void n_onMapReady (com.google.android.gms.maps.GoogleMap p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
