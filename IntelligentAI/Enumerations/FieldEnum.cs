using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

/// <summary>
/// ES搜索默认字段
/// </summary>
public class FieldEnum : Enumeration
{
    public static FieldEnum Article = new FieldEnum(0, nameof(Article), "articlesequenceid,paperid,papername,revision,page,paperdate,parenttitle,title,subtitle,editor,contenttxt,url,keyword,updatetime,createtime,itemtype,maineditor,createindex_time,region,viocesize,imagesource,imagecontent,markinfo,paperno,articletype,memo1,memo3,videosize,foreignuniquekey,contentwordscount,sourceurl,class1,videourl,imagesourcecount,watchcount,scenetags,literaturetype,tags,sameid3,place.ns,place.ns1,place.ns2,place.ns3,aiabs,aititle,aieventdate");

    public static FieldEnum Topic = new FieldEnum(1, nameof(Topic), "articlesequenceid,_score,degree,title,contenttxt,url,updatetime,papername,markinfo,class1,createtime,tags,same_id,sameid3,articletype,aiabs,aititle,aieventdate");

    public static FieldEnum Vector = new FieldEnum(2, nameof(Vector), "title,paperdate,fwtype,updatetime,mediaid,aid,paperid");

    public FieldEnum(int id, string name, string description) : base(id, name, description) { }

    public static FieldEnum GetById(int id) => FromId<FieldEnum>(id);

    public static FieldEnum GetByName(string name) => FromName<FieldEnum>(name);
}
