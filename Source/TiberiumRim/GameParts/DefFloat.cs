using System;
using System.Text.RegularExpressions;
using System.Xml;
using Verse;

namespace TR;

public class DefFloat<T> where T : Def
{
    public T def;
    public float value = 1;

    public DefFloat(){}

    public DefFloat(string def, float value)
    {
        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "def", def);
        this.value = value;
    }

    public DefFloat(T def, float value)
    {
        this.def = def;
        this.value = value;
    }

    public virtual void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        string s = Regex.Replace(xmlRoot.FirstChild.Value, @"\s+", "");
        string[] array = s.Split(',');
        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "def", array[0], null, null);
        if (array.Length > 1)
            this.value = (float) ParseHelper.FromString(array[1], typeof(float));
    }

    public override string ToString()
    {
        return def.LabelCap + "(" + value + ")";
    }

    public string ToStringPercent()
    {
        return def.LabelCap + "(" + value.ToStringPercent() + ")";
    }
}