using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public class CommentsMappings : Profile
{
    public CommentsMappings()
    {
        CreateMap<Comment, TreeNode>()
            .ForMember(dto => dto.Key, map => map.MapFrom(entity => entity.Id))
            .ForMember(dto => dto.Children, map => map.MapFrom(entity => entity.Children))
            .ForMember(dto => dto.Expanded, map => map.MapFrom(entity => true))
            .ForMember(dto => dto.Label, map => map.MapFrom(entity => entity.Message))
            .ForMember(dto => dto.Leaf, map => map.MapFrom(entity => entity.Children == null || entity.Children.Count() == 0))
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
    }
}
