using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.Enums;

public enum UserType
{
    Admin = 100000000,
    [Display(Name = "انباردار")]
    Inventory = 100000001,
    [Display(Name = "سایر بخش ها")]
    Other = 492930004,
    [Display(Name = "پشتیبانی")]
    Support = 492930000,
    [Display(Name = "کسب و کار")]
    BusinessCenter = 492930001
}