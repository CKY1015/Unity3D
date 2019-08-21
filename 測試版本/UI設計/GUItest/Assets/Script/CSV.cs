using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSV{
	static CSV csv;
	public List<string[]> m_ArrayData;

	public static CSV GetInstance(){
		if (csv == null) {
			csv = new CSV ();
		}
		return csv;
	}

	private CSV() {
		m_ArrayData = new List<string[]> ();
	}

	public string getString(int row, int col)
    {
		return m_ArrayData [row] [col];
	}

	public int getInt(int row, int col)
    {
		return int.Parse (m_ArrayData [row] [col]);
	}

    public double getDouble(int row, int col)
    {
        return double.Parse(m_ArrayData[row][col]);
    }



    public void loadFile(string path){
		m_ArrayData.Clear ();
		StreamReader sr = null;
		try{
			sr = File.OpenText(path);
			Debug.Log("file finded!");
		}
		catch{
			Debug.Log ("file don't finded!");
			return;
		}
		string line;
		while ((line = sr.ReadLine ()) != null) {
			m_ArrayData.Add (line.Split(','));
		}
		sr.Close ();
		sr.Dispose ();
	}
}
