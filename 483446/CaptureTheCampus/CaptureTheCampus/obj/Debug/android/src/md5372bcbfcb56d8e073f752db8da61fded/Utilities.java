package md5372bcbfcb56d8e073f752db8da61fded;


public class Utilities
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("CaptureTheCampus.Utilities, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Utilities.class, __md_methods);
	}


	public Utilities () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Utilities.class)
			mono.android.TypeManager.Activate ("CaptureTheCampus.Utilities, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public Utilities (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == Utilities.class)
			mono.android.TypeManager.Activate ("CaptureTheCampus.Utilities, CaptureTheCampus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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
