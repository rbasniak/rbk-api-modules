using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Comments
{
    public class TreeNode
    {
        public TreeNode()
        {
            Expanded = false;
        }

        public string Label { get; set; }
        public object Data { get; set; }
        public string Icon { get; set; }
        public string ExpandedIcon { get; set; }
        public string CollapsedIcon { get; set; }
        public bool Leaf { get; set; }
        public string Type { get; set; }
        public string Style { get; set; }
        public string StyleClass { get; set; }
        public bool Draggable { get; set; }
        public bool Droppable { get; set; }
        public bool Selectable { get; set; }
        public string Key { get; set; }
        public bool Expanded { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; }
    }

    public class CommentDetails : BaseDataTransferObject
    {
        public DateTime Date { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }

    public class CommentsMappings : Profile
    {
        public CommentsMappings()
        {
            CreateMap<Comment, TreeNode>()
                .ForMember(dto => dto.Key, map => map.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Children, map => map.MapFrom(entity => entity.Children))
                .ForMember(dto => dto.Expanded, map => map.MapFrom(entity => true))
                .ForMember(dto => dto.Label, map => map.MapFrom(entity => entity.Message))
                .ForMember(dto => dto.Parent, map => map.Ignore())
                .ForMember(dto => dto.Leaf, map => map.MapFrom(entity => entity.Children != null && entity.Children.Count() > 0))
                .ForMember(dto => dto.Data, map => map.MapFrom(entity => entity.Userdata))
                .ForMember(dto => dto.Selectable, map => map.MapFrom(entity => false))
                .ForMember(dto => dto.Icon, map => map.Ignore())
                .ForMember(dto => dto.ExpandedIcon, map => map.Ignore())
                .ForMember(dto => dto.CollapsedIcon, map => map.Ignore())
                .ForMember(dto => dto.Type, map => map.Ignore())
                .ForMember(dto => dto.Style, map => map.Ignore())
                .ForMember(dto => dto.StyleClass, map => map.Ignore())
                .ForMember(dto => dto.Draggable, map => map.Ignore())
                .ForMember(dto => dto.Droppable, map => map.Ignore());

            CreateMap<Comment, CommentDetails>()
                .ForMember(dto => dto.Avatar, map => map.Ignore());
        }
    }
}
