using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class VisualizationStyleEnum : Enumeration
{
    public static VisualizationStyleEnum Fact = new VisualizationStyleEnum(1, nameof(Fact), "写实");

    public static VisualizationStyleEnum Cartoon = new VisualizationStyleEnum(2, nameof(Cartoon), "卡通");

    public static VisualizationStyleEnum OldChinese = new VisualizationStyleEnum(3, nameof(OldChinese), "古风");

    public static VisualizationStyleEnum Cyberpunk = new VisualizationStyleEnum(4, nameof(Cyberpunk), "赛博朋克");

    public static VisualizationStyleEnum Watercolor = new VisualizationStyleEnum(5, nameof(Watercolor), "水彩画");

    public static VisualizationStyleEnum FantasyArt = new VisualizationStyleEnum(6, nameof(FantasyArt), "幻想艺术");

    public static VisualizationStyleEnum LineArt = new VisualizationStyleEnum(7, nameof(LineArt), "线条艺术");

    public static VisualizationStyleEnum OilPainting = new VisualizationStyleEnum(8, nameof(OilPainting), "油画");

    public static VisualizationStyleEnum OrigamiArt = new VisualizationStyleEnum(9, nameof(OrigamiArt), "折纸艺术");

    public static VisualizationStyleEnum Film = new VisualizationStyleEnum(10, nameof(Film), "电影胶片");

    public static VisualizationStyleEnum PixelArt = new VisualizationStyleEnum(11, nameof(PixelArt), "像素艺术");

    public static VisualizationStyleEnum Advertisement = new VisualizationStyleEnum(12, nameof(Advertisement), "广告");

    public static VisualizationStyleEnum AbstractArt = new VisualizationStyleEnum(13, nameof(AbstractArt), "抽象艺术");

    public static VisualizationStyleEnum Graffiti = new VisualizationStyleEnum(14, nameof(Graffiti), "涂鸦");

    public static VisualizationStyleEnum Surrealism = new VisualizationStyleEnum(15, nameof(Surrealism), "超现实主义");

    public static VisualizationStyleEnum PsychedelicArt = new VisualizationStyleEnum(16, nameof(PsychedelicArt), "迷幻艺术");

    public static VisualizationStyleEnum Renaissance = new VisualizationStyleEnum(17, nameof(Renaissance), "文艺复兴");

    public static VisualizationStyleEnum Print = new VisualizationStyleEnum(18, nameof(Print), "印刷");

    public static VisualizationStyleEnum Futuristic = new VisualizationStyleEnum(19, nameof(Futuristic), "未来摩登");

    public static VisualizationStyleEnum Cute = new VisualizationStyleEnum(20, nameof(Cute), "可爱风");

    public static VisualizationStyleEnum ClassicalArt = new VisualizationStyleEnum(21, nameof(ClassicalArt), "古典艺术");

    public static VisualizationStyleEnum Cubism = new VisualizationStyleEnum(22, nameof(Cubism), "立体主义");

    public static VisualizationStyleEnum DarkFantasy = new VisualizationStyleEnum(23, nameof(DarkFantasy), "黑暗幻想");

    public static VisualizationStyleEnum Flat2D = new VisualizationStyleEnum(24, nameof(Flat2D), "平面2D");

    public static VisualizationStyleEnum InkWashPainting = new VisualizationStyleEnum(25, nameof(InkWashPainting), "滴墨画");

    public static VisualizationStyleEnum LogoDesign = new VisualizationStyleEnum(26, nameof(LogoDesign), "Logo设计");

    public static VisualizationStyleEnum PencilSketch = new VisualizationStyleEnum(27, nameof(PencilSketch), "铅笔素描");

    public static VisualizationStyleEnum SilhouetteArt = new VisualizationStyleEnum(28, nameof(SilhouetteArt), "剪影艺术");

    public static VisualizationStyleEnum SketchMaster = new VisualizationStyleEnum(29, nameof(SketchMaster), "草图大师");

    public static VisualizationStyleEnum Steampunk = new VisualizationStyleEnum(30, nameof(Steampunk), "蒸汽朋克");

    public static VisualizationStyleEnum StickerArt = new VisualizationStyleEnum(31, nameof(StickerArt), "贴纸艺术");

    public VisualizationStyleEnum(int id, string name, string description) : base(id, name, description) { }

    public static VisualizationStyleEnum GetById(int id) => FromId<VisualizationStyleEnum>(id);

    public static VisualizationStyleEnum GetByName(string name) => FromName<VisualizationStyleEnum>(name);

    public static VisualizationStyleEnum GetByDescription(string description) => FromDescription<VisualizationStyleEnum>(description);
}
