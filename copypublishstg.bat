copy ..\Publish\*.dll C:\OBC_PUBLISH\OBC_CORE_API_STG\
copy ..\Publish\*.pdb C:\OBC_PUBLISH\OBC_CORE_API_STG\
copy ..\Publish\*.deps.json C:\OBC_PUBLISH\OBC_CORE_API_STG\


                   


CREATE TABLE [dbo].[tnNhanVien_test]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[MaSoNV] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[HoTenNV] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DienThoai] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SoNB] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DiaChi] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Email] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MatKhau] [nvarchar] (300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MaPB] [int] NULL,
[MaCV] [int] NULL,
[MaTN] [tinyint] NULL,
[IsSuperAdmin] [bit] NULL,
[SLThongBao] [int] NULL,
[IsLocked] [bit] NULL,
[NgaySinh] [datetime] NULL,
[AvatarUrl] [nvarchar] (350) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AmountNotify] [int] NULL,
[AmountProposal] [int] NULL,
[AmountChecklist] [int] NULL,
[AmountRequest] [int] NULL,
[IdZalo] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsZalo] [bit] NULL,
[NameZalo] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsHidden] [bit] NULL,
[CreatedWhen] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedWhen] [datetime] NULL,
[ModifiedBy] [int] NULL,
[IsEnabled] [bit] NULL,
[ConcurrencyStamp] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PasswordHash] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MaNV] [int] NULL
)