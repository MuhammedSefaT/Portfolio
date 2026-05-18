using AutoMapper;
using Portfolio.Common.Dtos;
using Portfolio.Entity;

namespace Portfolio.Business.Mapping;

public class CertificateProfile : Profile
{
    public CertificateProfile()
    {
        CreateMap<Certificate, CertificateCreateDto>().ReverseMap();
        CreateMap<Certificate, CertificateUpdateDto>().ReverseMap();
        CreateMap<Certificate, CertificateListDto>().ReverseMap();
    }
}
