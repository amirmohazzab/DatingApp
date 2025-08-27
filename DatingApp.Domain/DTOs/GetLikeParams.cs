using DatingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class GetLikeParams : BasePagination
    {
        public PredicateLikeEnum PredicateUserLike { get; set; }
    }
}
