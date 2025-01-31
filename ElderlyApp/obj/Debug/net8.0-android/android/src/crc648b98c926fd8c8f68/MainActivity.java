package crc648b98c926fd8c8f68;


public class MainActivity
	extends crc6488302ad6e9e4df1a.MauiAppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("ElderlyApp.MainActivity, ElderlyApp", MainActivity.class, __md_methods);
	}


	public MainActivity ()
	{
		super ();
		if (getClass () == MainActivity.class) {
			mono.android.TypeManager.Activate ("ElderlyApp.MainActivity, ElderlyApp", "", this, new java.lang.Object[] {  });
		}
	}


	public MainActivity (int p0)
	{
		super (p0);
		if (getClass () == MainActivity.class) {
			mono.android.TypeManager.Activate ("ElderlyApp.MainActivity, ElderlyApp", "System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0 });
		}
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
