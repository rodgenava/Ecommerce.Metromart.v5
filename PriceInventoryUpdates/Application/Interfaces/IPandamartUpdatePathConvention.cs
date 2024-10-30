﻿using Data.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IPandamartUpdatePathConvention
    {
        string Localize(Warehouse warehouse, PandamartStore pandamartStore);
    }
}