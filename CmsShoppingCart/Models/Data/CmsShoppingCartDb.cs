﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CmsShoppingCart.Models.Data
{
    public class CmsShoppingCartDb : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
    }
}