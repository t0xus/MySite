﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MySite.Models;

public partial class ResumeContent
{
    public int Id { get; set; }

    public string PositionTitle { get; set; }

    public string Place { get; set; }

    public string Company { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTill { get; set; }

    public short? IdRl { get; set; }

    public string FullText { get; set; }
}