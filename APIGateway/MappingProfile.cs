﻿using AutoMapper;
using APIGateway.Model;
using ClientService;
using ServiceMessage;
using ServiceChat;
using Google.Protobuf.WellKnownTypes;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ClientMessage, Client>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.userName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.friends, opt => opt.MapFrom(src => src.FriendsIds != null ? src.FriendsIds.Select(f => f.ToString()).ToArray() : null))
            .ForMember(dest => dest.chats, opt => opt.MapFrom(src => src.ChatIds != null ? src.ChatIds.Select(c => c.ToString()).ToArray() : null));

        CreateMap<Message, MessageModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ToString()))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo != null ? src.Photo : null));

        CreateMap<Chat, ChatModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Chaters, opt => opt.MapFrom(src => src.Chaters.Select(c => c.ToString()).ToArray()))
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages != null ? src.Messages.Select(m => m.ToString()).ToArray() : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToDateTime().ToString("o")))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt != null ? src.UpdatedAt.ToDateTime().ToString("o") : null));
    }
}