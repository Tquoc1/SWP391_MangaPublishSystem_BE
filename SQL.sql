USE [master];
GO

/* ==========================================
   BƯỚC 1: XÓA DATABASE CŨ NẾU ĐANG TỒN TẠI
   ========================================== */
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MangaPublishDB')
BEGIN
    -- Ép ngắt toàn bộ kết nối hiện tại (Postman, Backend đang chạy) để tránh lỗi Database đang bận
    ALTER DATABASE [MangaPublishDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Xóa bỏ hoàn toàn database cũ
    DROP DATABASE [MangaPublishDB];
END
GO

/* ==========================================
   BƯỚC 2: TẠO MỚI DATABASE VÀ CẤU HÌNH ANSI
   ========================================== */
CREATE DATABASE [MangaPublishDB];
GO
GO

USE [MangaPublishDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ==========================================
   GROUP 1: CÁC BẢNG ĐỘC LẬP (Tạo trước)
   ========================================== */

-- 1. Bảng Roles
CREATE TABLE [dbo].[roles](
	[roleid] [int] IDENTITY(1,1) NOT NULL,
	[rolename] [nvarchar](255) NOT NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [roles_pkey] PRIMARY KEY CLUSTERED ([roleid] ASC),
 UNIQUE NONCLUSTERED ([rolename] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/* ==========================================
   GROUP 2: BẢNG USERS & CÁC PROFILE LIÊN QUAN
   ========================================== */

-- 2. Bảng Users
CREATE TABLE [dbo].[users](
	[userid] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](255) NOT NULL,
	[passwordhash] [nvarchar](255) NOT NULL,
	[fullname] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NULL,
	[roleid] [int] NOT NULL,
	[createdat] [datetime] NULL DEFAULT (getdate()),
	[isdeleted] [bit] NULL DEFAULT ((0)),
 CONSTRAINT [users_pkey] PRIMARY KEY CLUSTERED ([userid] ASC),
 UNIQUE NONCLUSTERED ([email] ASC),
 UNIQUE NONCLUSTERED ([username] ASC)
) ON [PRIMARY]
GO

-- 3. Bảng Assistant Profiles
CREATE TABLE [dbo].[assistant_profiles](
	[assistant_profile_id] [int] IDENTITY(1,1) NOT NULL,
	[userid] [int] NOT NULL,
	[avatar_url] [nvarchar](max) NULL,
	[portfolio_url] [nvarchar](max) NULL,
	[phone_number] [nvarchar](50) NULL,
	[is_available] [bit] NULL DEFAULT ((1)),
	[skills] [nvarchar](max) NULL,
	[software_used] [nvarchar](max) NULL,
	[bank_name] [nvarchar](100) NULL,
	[bank_account_number] [nvarchar](50) NULL,
	[bank_account_name] [nvarchar](255) NULL,
	[updatedat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [assistant_profiles_pkey] PRIMARY KEY CLUSTERED ([assistant_profile_id] ASC),
 UNIQUE NONCLUSTERED ([userid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 4. Bảng Mangaka Profiles
CREATE TABLE [dbo].[mangaka_profiles](
	[mangaka_profile_id] [int] IDENTITY(1,1) NOT NULL,
	[userid] [int] NOT NULL,
	[pen_name] [nvarchar](255) NOT NULL,
	[avatar_url] [nvarchar](max) NULL,
	[bio] [nvarchar](max) NULL,
	[phone_number] [nvarchar](50) NULL,
	[bank_name] [nvarchar](100) NULL,
	[bank_account_number] [nvarchar](50) NULL,
	[bank_account_name] [nvarchar](255) NULL,
	[updatedat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [mangaka_profiles_pkey] PRIMARY KEY CLUSTERED ([mangaka_profile_id] ASC),
 UNIQUE NONCLUSTERED ([userid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 5. Bảng Hợp đồng Mangaka & Assistant
CREATE TABLE [dbo].[mangaka_assistants](
	[contract_id] [int] IDENTITY(1,1) NOT NULL,
	[mangaka_id] [int] NOT NULL,
	[assistant_id] [int] NOT NULL,
	[salary_amount] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[salary_type] [nvarchar](50) NULL DEFAULT ('Monthly'),
	[contract_terms] [nvarchar](max) NULL,
	[status] [nvarchar](50) NULL DEFAULT ('Pending'),
	[start_date] [date] NULL,
	[end_date] [date] NULL,
	[createdat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [mangaka_assistants_pkey] PRIMARY KEY CLUSTERED ([contract_id] ASC),
 CONSTRAINT [unique_active_contract] UNIQUE NONCLUSTERED ([mangaka_id] ASC, [assistant_id] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 6. Bảng User Tokens (Dùng cho Auth/Refresh Token)
CREATE TABLE [dbo].[user_tokens](
	[tokenid] [int] IDENTITY(1,1) NOT NULL,
	[userid] [int] NOT NULL,
	[token] [nvarchar](500) NOT NULL,
	[isrevoked] [bit] NULL DEFAULT ((0)),
	[expiresat] [datetime] NOT NULL,
 PRIMARY KEY CLUSTERED ([tokenid] ASC),
 UNIQUE NONCLUSTERED ([token] ASC)
) ON [PRIMARY]
GO

/* ==========================================
   GROUP 3: TRUYỆN TRANH (SERIES -> CHAPTER -> PAGE)
   ========================================== */

-- 7. Bảng Series (Tác phẩm)
CREATE TABLE [dbo].[series](
	[seriesid] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[synopsis] [nvarchar](max) NULL,
	[mangakaid] [int] NOT NULL,
	[tantoueditorid] [int] NOT NULL,
	[publishformat] [nvarchar](50) NULL,
	[status] [nvarchar](50) NULL,
	[proposalfileurl] [nvarchar](max) NULL,
	[createdat] [datetime] NULL DEFAULT (getdate()),
	[approvedat] [datetime] NULL,
	[isdeleted] [bit] NULL DEFAULT ((0)),
 CONSTRAINT [series_pkey] PRIMARY KEY CLUSTERED ([seriesid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 8. Bảng Chapters (Chương truyện)
CREATE TABLE [dbo].[chapters](
	[chapterid] [int] IDENTITY(1,1) NOT NULL,
	[seriesid] [int] NOT NULL,
	[chapternumber] [int] NOT NULL,
	[title] [nvarchar](255) NULL,
	[deadline] [datetime] NOT NULL,
	[status] [nvarchar](50) NULL,
	[createdat] [datetime] NULL DEFAULT (getdate()),
	[isdeleted] [bit] NULL DEFAULT ((0)),
 CONSTRAINT [chapters_pkey] PRIMARY KEY CLUSTERED ([chapterid] ASC)
) ON [PRIMARY]
GO

-- 9. Bảng Pages (Trang truyện)
CREATE TABLE [dbo].[pages](
	[pageid] [int] IDENTITY(1,1) NOT NULL,
	[chapterid] [int] NOT NULL,
	[pagenumber] [int] NOT NULL,
	[status] [nvarchar](50) NULL DEFAULT ('Draft'),
	[isdeleted] [bit] NULL DEFAULT ((0)),
	[pageimageurl] [varchar](500) NULL,
 CONSTRAINT [pages_pkey] PRIMARY KEY CLUSTERED ([pageid] ASC)
) ON [PRIMARY]
GO

/* ==========================================
   GROUP 4: LAYER, ISSUE & ĐÁNH GIÁ (Bảng con hạ nguồn)
   ========================================== */

-- 10. Bảng Pagelayers (Lớp bản vẽ - Đã dọn dẹp issueid)
CREATE TABLE [dbo].[pagelayers](
	[layerid] [int] IDENTITY(1,1) NOT NULL,
	[pageid] [int] NOT NULL,
	[uploaderid] [int] NOT NULL,
	[layername] [nvarchar](255) NULL,
	[fileurl] [nvarchar](max) NOT NULL,
	[zindex] [int] NULL DEFAULT ((0)),
	[versionnumber] [int] NULL DEFAULT ((1)),
	[isvisible] [bit] NULL DEFAULT ((1)),
	[createdat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [pagelayers_pkey] PRIMARY KEY CLUSTERED ([layerid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 11. Bảng Page Issues (Bắt lỗi trang truyện)
CREATE TABLE [dbo].[page_issues](
	[issueid] [int] IDENTITY(1,1) NOT NULL,
	[pageid] [int] NOT NULL,
	[created_by_id] [int] NOT NULL,
	[assigned_to_id] [int] NULL,
	[issue_type] [nvarchar](50) NOT NULL,
	[work_category] [nvarchar](50) NULL,
	[box_x] [int] NOT NULL DEFAULT ((0)),
	[box_y] [int] NOT NULL DEFAULT ((0)),
	[box_width] [int] NOT NULL DEFAULT ((0)),
	[box_height] [int] NOT NULL DEFAULT ((0)),
	[description] [nvarchar](max) NOT NULL,
	[status] [nvarchar](50) NULL DEFAULT ('Pending'),
	[deadline] [datetime] NULL,
	[createdat] [datetime] NULL DEFAULT (getdate()),
	[completedat] [datetime] NULL,
	[isdeleted] [bit] NULL DEFAULT ((0)),
 CONSTRAINT [page_issues_pkey] PRIMARY KEY CLUSTERED ([issueid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 12. Bảng Issue Resources (Tài liệu đính kèm lỗi)
CREATE TABLE [dbo].[issue_resources](
	[resourceid] [int] IDENTITY(1,1) NOT NULL,
	[issueid] [int] NOT NULL,
	[filename] [nvarchar](255) NOT NULL,
	[fileurl] [nvarchar](max) NOT NULL,
	[uploadedat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [issue_resources_pkey] PRIMARY KEY CLUSTERED ([resourceid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 13. Bảng Board Evaluations (Đánh giá của hội đồng)
CREATE TABLE [dbo].[board_evaluations](
	[evaluationid] [int] IDENTITY(1,1) NOT NULL,
	[seriesid] [int] NOT NULL,
	[inputtedbyid] [int] NOT NULL,
	[story_score] [decimal](4, 2) NOT NULL DEFAULT ((0)),
	[art_score] [decimal](4, 2) NOT NULL DEFAULT ((0)),
	[character_score] [decimal](4, 2) NOT NULL DEFAULT ((0)),
	[commercial_score] [decimal](4, 2) NOT NULL DEFAULT ((0)),
	[pacing_score] [decimal](4, 2) NOT NULL DEFAULT ((0)),
	[average_score] AS (CONVERT([decimal](4,2),(((([story_score]+[art_score])+[character_score])+[commercial_score])+[pacing_score])/(5.0))),
	[final_decision] [nvarchar](50) NULL,
	[approved_publish_format] [nvarchar](50) NULL,
	[feedback] [nvarchar](max) NULL,
	[evaluatedat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [board_evaluations_pkey] PRIMARY KEY CLUSTERED ([evaluationid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 14. Bảng Notifications (Thông báo hệ thống)
CREATE TABLE [dbo].[notifications](
	[notificationid] [int] IDENTITY(1,1) NOT NULL,
	[userid] [int] NOT NULL,
	[seriesid] [int] NULL,
	[title] [nvarchar](255) NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[isread] [bit] NULL DEFAULT ((0)),
	[createdat] [datetime] NULL DEFAULT (getdate()),
 CONSTRAINT [notifications_pkey] PRIMARY KEY CLUSTERED ([notificationid] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 15. Bảng Weekly Rankings (Bảng xếp hạng tuần)
CREATE TABLE [dbo].[weekly_rankings](
	[rankingid] [int] IDENTITY(1,1) NOT NULL,
	[seriesid] [int] NOT NULL,
	[issue_number] [int] NOT NULL,
	[issue_year] [int] NOT NULL,
	[votecount] [int] NULL DEFAULT ((0)),
	[inputtedbyid] [int] NOT NULL,
	[recordedat] [datetime] NULL DEFAULT (getdate()),
	[rankposition] [int] NULL,
	[isbottomrank] [bit] NULL DEFAULT ((0)),
	[calculatedat] [datetime] NULL,
 CONSTRAINT [weekly_rankings_pkey] PRIMARY KEY CLUSTERED ([rankingid] ASC),
 CONSTRAINT [unique_series_weekly_evaluation] UNIQUE NONCLUSTERED ([seriesid] ASC, [issue_number] ASC, [issue_year] ASC)
) ON [PRIMARY]
GO


/* ==========================================
   GROUP 5: RÀNG BUỘC KHÓA NGOẠI (FOREIGN KEYS)
   ========================================== */

ALTER TABLE [dbo].[users] WITH CHECK ADD CONSTRAINT [users_roleid_fkey] FOREIGN KEY([roleid])
REFERENCES [dbo].[roles] ([roleid])
GO

ALTER TABLE [dbo].[assistant_profiles] WITH CHECK ADD CONSTRAINT [assistant_profiles_userid_fkey] FOREIGN KEY([userid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[mangaka_profiles] WITH CHECK ADD CONSTRAINT [mangaka_profiles_userid_fkey] FOREIGN KEY([userid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[mangaka_assistants] WITH CHECK ADD CONSTRAINT [mangaka_assistants_assistant_fkey] FOREIGN KEY([assistant_id])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[mangaka_assistants] WITH CHECK ADD CONSTRAINT [mangaka_assistants_mangaka_fkey] FOREIGN KEY([mangaka_id])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[user_tokens] WITH CHECK ADD FOREIGN KEY([userid])
REFERENCES [dbo].[users] ([userid]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[series] WITH CHECK ADD CONSTRAINT [series_mangakaid_fkey] FOREIGN KEY([mangakaid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[series] WITH CHECK ADD CONSTRAINT [series_tantoueditorid_fkey] FOREIGN KEY([tantoueditorid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[chapters] WITH CHECK ADD CONSTRAINT [chapters_seriesid_fkey] FOREIGN KEY([seriesid])
REFERENCES [dbo].[series] ([seriesid])
GO

ALTER TABLE [dbo].[pages] WITH CHECK ADD CONSTRAINT [pages_chapterid_fkey] FOREIGN KEY([chapterid])
REFERENCES [dbo].[chapters] ([chapterid])
GO

ALTER TABLE [dbo].[pagelayers] WITH CHECK ADD CONSTRAINT [pagelayers_pageid_fkey] FOREIGN KEY([pageid])
REFERENCES [dbo].[pages] ([pageid])
GO

ALTER TABLE [dbo].[pagelayers] WITH CHECK ADD CONSTRAINT [pagelayers_uploaderid_fkey] FOREIGN KEY([uploaderid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CONSTRAINT [page_issues_pageid_fkey] FOREIGN KEY([pageid])
REFERENCES [dbo].[pages] ([pageid])
GO

ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CONSTRAINT [page_issues_created_by_fkey] FOREIGN KEY([created_by_id])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CONSTRAINT [page_issues_assigned_to_fkey] FOREIGN KEY([assigned_to_id])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[issue_resources] WITH CHECK ADD CONSTRAINT [issue_resources_issueid_fkey] FOREIGN KEY([issueid])
REFERENCES [dbo].[page_issues] ([issueid])
GO

ALTER TABLE [dbo].[board_evaluations] WITH CHECK ADD CONSTRAINT [board_evaluations_inputtedbyid_fkey] FOREIGN KEY([inputtedbyid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[board_evaluations] WITH CHECK ADD CONSTRAINT [board_evaluations_seriesid_fkey] FOREIGN KEY([seriesid])
REFERENCES [dbo].[series] ([seriesid])
GO

ALTER TABLE [dbo].[notifications] WITH CHECK ADD CONSTRAINT [notifications_seriesid_fkey] FOREIGN KEY([seriesid])
REFERENCES [dbo].[series] ([seriesid])
GO

ALTER TABLE [dbo].[notifications] WITH CHECK ADD CONSTRAINT [notifications_userid_fkey] FOREIGN KEY([userid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[weekly_rankings] WITH CHECK ADD CONSTRAINT [weekly_rankings_inputtedbyid_fkey] FOREIGN KEY([inputtedbyid])
REFERENCES [dbo].[users] ([userid])
GO

ALTER TABLE [dbo].[weekly_rankings] WITH CHECK ADD CONSTRAINT [weekly_rankings_seriesid_fkey] FOREIGN KEY([seriesid])
REFERENCES [dbo].[series] ([seriesid])
GO


/* ==========================================
   GROUP 6: RÀNG BUỘC KIỂM TRA DỮ LIỆU (CHECK CONSTRAINTS)
   ========================================== */

ALTER TABLE [dbo].[board_evaluations] WITH CHECK ADD CHECK (([approved_publish_format]='Monthly' OR [approved_publish_format]='Weekly'))
GO
ALTER TABLE [dbo].[board_evaluations] WITH CHECK ADD CHECK (([final_decision]='Reject' OR [final_decision]='Approve'))
GO
ALTER TABLE [dbo].[chapters] WITH CHECK ADD CHECK (([status]='Delayed' OR [status]='Published' OR [status]='ReadyForPrint' OR [status]='EditorReviewing' OR [status]='StudioWorking' OR [status]='Drafting'))
GO
ALTER TABLE [dbo].[mangaka_assistants] WITH CHECK ADD CHECK (([salary_type]='Fixed' OR [salary_type]='PerChapter' OR [salary_type]='Monthly'))
GO
ALTER TABLE [dbo].[mangaka_assistants] WITH CHECK ADD CHECK (([status]='Terminated' OR [status]='Active' OR [status]='Pending'))
GO
ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CHECK (([issue_type]='Revision' OR [issue_type]='Production'))
GO
ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CHECK (([status]='Approved' OR [status]='NeedsRevision' OR [status]='Submitted' OR [status]='InProgress' OR [status]='Pending'))
GO
ALTER TABLE [dbo].[page_issues] WITH CHECK ADD CHECK (([work_category]='Content' OR [work_category]='Dialog' OR [work_category]='Inking' OR [work_category]='Effects' OR [work_category]='Shading' OR [work_category]='Background'))
GO
ALTER TABLE [dbo].[pages] WITH CHECK ADD CHECK (([status]='Approved' OR [status]='Reviewing' OR [status]='InWork' OR [status]='Draft'))
GO
ALTER TABLE [dbo].[series] WITH CHECK ADD CHECK (([publishformat]='Pending' OR [publishformat]='Monthly' OR [publishformat]='Weekly'))
GO

	ALTER TABLE [dbo].[series]
ADD 
    [coverimageurl] [nvarchar](500) NULL,
    [agerating] [nvarchar](10) NOT NULL DEFAULT 'G',
    CONSTRAINT [chk_series_agerating] CHECK ([agerating] IN ('G', 'PG-13', 'R-16', 'R-18'))
GO



-- Tạo bảng danh mục Thể loại gốc
CREATE TABLE [dbo].[genres](
    [genreid] [int] IDENTITY(1,1) NOT NULL,
    [genrename] [nvarchar](100) NOT NULL,
    [description] [nvarchar](255) NULL,
 CONSTRAINT [genres_pkey] PRIMARY KEY CLUSTERED ([genreid] ASC)
) ON [PRIMARY]
GO

-- Tạo bảng trung gian kết nối Nhiều - Nhiều giữa Series và Genres
CREATE TABLE [dbo].[series_genres](
    [seriesid] [int] NOT NULL,
    [genreid] [int] NOT NULL,
 CONSTRAINT [series_genres_pkey] PRIMARY KEY CLUSTERED ([seriesid] ASC, [genreid] ASC),
 CONSTRAINT [fk_sg_series] FOREIGN KEY ([seriesid]) REFERENCES [dbo].[series] ([seriesid]) ON DELETE CASCADE,
 CONSTRAINT [fk_sg_genres] FOREIGN KEY ([genreid]) REFERENCES [dbo].[genres] ([genreid]) ON DELETE CASCADE
) ON [PRIMARY]
GO


-- Tạo bảng danh mục Tags gốc
CREATE TABLE [dbo].[tags](
    [tagid] [int] IDENTITY(1,1) NOT NULL,
    [tagname] [nvarchar](50) NOT NULL,
 CONSTRAINT [tags_pkey] PRIMARY KEY CLUSTERED ([tagid] ASC)
) ON [PRIMARY]
GO

-- Tạo bảng trung gian kết nối Nhiều - Nhiều giữa Series và Tags
CREATE TABLE [dbo].[series_tags](
    [seriesid] [int] NOT NULL,
    [tagid] [int] NOT NULL,
 CONSTRAINT [series_tags_pkey] PRIMARY KEY CLUSTERED ([seriesid] ASC, [tagid] ASC),
 CONSTRAINT [fk_st_series] FOREIGN KEY ([seriesid]) REFERENCES [dbo].[series] ([seriesid]) ON DELETE CASCADE,
 CONSTRAINT [fk_st_tags] FOREIGN KEY ([tagid]) REFERENCES [dbo].[tags] ([tagid]) ON DELETE CASCADE
) ON [PRIMARY]
GO
	
SET IDENTITY_INSERT [dbo].[roles] ON;
GO

DELETE FROM [dbo].[roles] WHERE [roleid] IN (1, 2, 3, 4, 5);
GO

INSERT [dbo].[roles] ([roleid], [rolename], [description]) VALUES 
(1, N'Admin', N'Quản trị viên toàn hệ thống'),
(2, N'Editorial Board', N'Tổng biên tập - Duyệt phát hành, phân bổ đầu truyện...'),
(3, N'Tantou Editor', N'Biên tập viên (Tantou) - Theo dõi trực tiếp tác giả'),
(4, N'Mangaka', N'Họa sĩ chính / Tác giả truyện tranh'),
(5, N'Assistant', N'Trợ lý họa sĩ - Nhận task phụ trợ (tô màu, đi nét...)');
GO


SET IDENTITY_INSERT [dbo].[roles] OFF;
GO


