using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class LibraryEnum : Enumeration
{
    public static LibraryEnum FanewsLibrary = new LibraryEnum(0, nameof(FanewsLibrary), "凡闻全库");

    public static LibraryEnum ForestryLibrary = new LibraryEnum(1, nameof(ForestryLibrary), "林业库");

    public static LibraryEnum RegulationLibrary = new LibraryEnum(519, nameof(RegulationLibrary), "法规库");

    public static LibraryEnum XjpLibrary = new LibraryEnum(520, nameof(XjpLibrary), "习近平专题库");

    public static LibraryEnum XjpYuluLibrary = new LibraryEnum(1750, nameof(XjpYuluLibrary), "习近平语录库");

    public static LibraryEnum OpinionLibrary = new LibraryEnum(1751, nameof(OpinionLibrary), "观点库");

    public static LibraryEnum OlympicLibrary = new LibraryEnum(1752, nameof(OlympicLibrary), "奥运库");

    public LibraryEnum(int id, string name, string description) : base(id, name, description) { }

    public static LibraryEnum GetById(int id) => FromId<LibraryEnum>(id);
    public static LibraryEnum GetByName(string name) => FromName<LibraryEnum>(name);
    public static LibraryEnum GetByDescription(string description) => FromDescription<LibraryEnum>(description);
}