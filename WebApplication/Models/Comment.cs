﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TopicId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
