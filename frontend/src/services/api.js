const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7082/api'; // Update with your actual BE URL

/**
 * Helper to get the JWT token from storage
 */
const getToken = () => localStorage.getItem('token');

/**
 * Standardized request helper
 */
async function request(endpoint, options = {}) {
  const token = getToken();
  
  const headers = {
    ...options.headers,
  };

  // Only set Content-Type to application/json if we are not sending FormData
  if (!(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const config = {
    ...options,
    headers,
  };

  const response = await fetch(`${BASE_URL}${endpoint}`, config);

  if (response.status === 401) {
    // Optional: Handle token refresh or redirect to login here
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.dispatchEvent(new Event('auth-expired'));
  }

  if (!response.ok) {
    let errorMessage = 'Something went wrong';
    try {
      const errorData = await response.json();
      errorMessage = errorData.message || errorData.Message || JSON.stringify(errorData);
    } catch {
      try {
        errorMessage = await response.text();
      } catch {}
    }
    throw new Error(errorMessage || `Request failed with status ${response.status}`);
  }

  // Handle empty responses (like 204 NoContent)
  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const api = {
  // === AUTHENTICATION ===
  auth: {
    login: (credentials) => request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    }),
    register: (userData) => request('/auth/register', {
      method: 'POST',
      body: JSON.stringify(userData),
    }),
    refreshToken: (tokens) => request('/auth/refresh-token', {
      method: 'POST',
      body: JSON.stringify(tokens),
    }),
    logout: (tokens) => request('/auth/logout', {
      method: 'POST',
      body: JSON.stringify(tokens),
    }),
    createStaff: (staffData) => request('/auth/create-staff', {
      method: 'POST',
      body: JSON.stringify(staffData),
    }),
  },

  // === ADMIN USERS ===
  admin: {
    getUsers: (params = {}) => {
      const query = new URLSearchParams();
      if (params.roleId) query.append('roleId', params.roleId);
      if (params.status) query.append('status', params.status);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/admin/users${queryString}`);
    },
    getUserDetail: (id) => request(`/admin/users/${id}`),
    createUser: (userData) => request('/admin/users', {
      method: 'POST',
      body: JSON.stringify(userData),
    }),
    updateUser: (id, userData) => request(`/admin/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(userData),
    }),
    updateStatus: (id, statusData) => request(`/admin/users/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(statusData),
    }),
    deleteUser: (id) => request(`/admin/users/${id}`, {
      method: 'DELETE',
    }),
  },

  // === USERS & PROFILE ===
  users: {
    getProfile: () => request('/users/profile'),
    updateMangakaProfile: (formData) => request('/users/profile/mangaka', {
      method: 'PUT',
      body: formData, // FormData containing fields and avatarFile
    }),
    updateAssistantProfile: (formData) => request('/users/profile/assistant', {
      method: 'PUT',
      body: formData, // FormData containing fields and avatarFile
    }),
    getAvailableAssistants: () => request('/users/available-assistants'),
    getTantouEditors: () => request('/users/tantou-editors'),
    getEvaluators: () => request('/users/evaluators'),
  },

  // === SERIES ===
  series: {
    getAll: (params = {}) => {
      const query = new URLSearchParams();
      if (params.mangakaId) query.append('mangakaId', params.mangakaId);
      if (params.status) query.append('status', params.status);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/Series${queryString}`);
    },
    getById: (id) => request(`/Series/${id}`),
    getByMangakaId: (mangakaId) => request(`/Series/mangakaid/${mangakaId}`),
    create: (formData) => request('/Series', {
      method: 'POST',
      body: formData, // FormData containing metadata and proposalFile, coverImage
    }),
    update: (id, formData) => request(`/Series/${id}`, {
      method: 'PUT',
      body: formData, // FormData containing metadata and optional files
    }),
    updateStatus: (id, statusDto) => request(`/Series/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(statusDto),
    }),
    updatePublishFormat: (id, formatDto) => request(`/Series/${id}/publish-format`, {
      method: 'PATCH',
      body: JSON.stringify(formatDto),
    }),
    updateTantouEditor: (id, updateDto) => request(`/Series/${id}/tantou-editor`, {
      method: 'PATCH',
      body: JSON.stringify(updateDto),
    }),
    softDelete: (id) => request(`/Series/softdelete/${id}`, {
      method: 'DELETE',
    }),
  },

  // === CHAPTERS ===
  chapters: {
    getAll: (params = {}) => {
      const query = new URLSearchParams();
      if (params.seriesId) query.append('seriesId', params.seriesId);
      if (params.status) query.append('status', params.status);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/Chapters${queryString}`);
    },
    getByAssistantId: (assistantId) => request(`/Chapters/assistant/${assistantId}`),
    getById: (id) => request(`/Chapters/${id}`),
    create: (chapterData) => request('/Chapters', {
      method: 'POST',
      body: JSON.stringify(chapterData),
    }),
    update: (id, chapterData) => request(`/Chapters/${id}`, {
      method: 'PUT',
      body: JSON.stringify(chapterData),
    }),
    updateStatus: (id, status) => request(`/Chapters/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(status),
    }),
    softDelete: (id) => request(`/Chapters/${id}/soft`, {
      method: 'DELETE',
    }),
  },

  // === PAGES ===
  pages: {
    getAll: (params = {}) => {
      const query = new URLSearchParams();
      if (params.chapterId) query.append('chapterId', params.chapterId);
      if (params.status) query.append('status', params.status);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/Pages${queryString}`);
    },
    getById: (id) => request(`/Pages/${id}`),
    create: (formData) => request('/Pages', {
      method: 'POST',
      body: formData, // FormData with pageDto metadata and pageFile
    }),
    update: (id, pageUpdateDto) => request(`/Pages/${id}`, {
      method: 'PUT',
      body: JSON.stringify(pageUpdateDto),
    }),
    compositeImage: (id) => request(`/Pages/${id}/composite`, {
      method: 'POST',
    }),
    updateStatus: (id, status) => request(`/Pages/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(status),
    }),
    softDelete: (id) => request(`/Pages/${id}/soft`, {
      method: 'DELETE',
    }),
  },

  // === PAGE LAYERS ===
  pageLayers: {
    getAll: (pageId) => {
      const queryString = pageId ? `?pageId=${pageId}` : '';
      return request(`/PageLayers${queryString}`);
    },
    getById: (id) => request(`/PageLayers/${id}`),
    create: (formData) => request('/PageLayers', {
      method: 'POST',
      body: formData, // FormData with Layer details and layerFile
    }),
    update: (id, formData) => request(`/PageLayers/${id}`, {
      method: 'PUT',
      body: formData, // FormData with optional layerFile
    }),
    softDelete: (id) => request(`/PageLayers/${id}/soft`, {
      method: 'DELETE',
    }),
  },

  // === PAGE ISSUES ===
  pageIssues: {
    getAll: (params = {}) => {
      const query = new URLSearchParams();
      if (params.chapterId) query.append('chapterId', params.chapterId);
      if (params.pageId) query.append('pageId', params.pageId);
      if (params.status) query.append('status', params.status);
      if (params.workCategory) query.append('workCategory', params.workCategory);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/PageIssues${queryString}`);
    },
    getById: (id) => request(`/PageIssues/${id}`),
    create: (issueData) => request('/PageIssues', {
      method: 'POST',
      body: JSON.stringify(issueData),
    }),
    updateStatus: (id, statusDto) => request(`/PageIssues/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(statusDto),
    }),
    update: (id, issueData) => request(`/PageIssues/${id}`, {
      method: 'PUT',
      body: JSON.stringify(issueData),
    }),
    softDelete: (id) => request(`/PageIssues/${id}/soft`, {
      method: 'DELETE',
    }),
  },

  // === MANGAKA ASSISTANT CONTRACTS ===
  mangakaAssistant: {
    getAll: (params = {}) => {
      const query = new URLSearchParams();
      if (params.mangakaId) query.append('mangakaId', params.mangakaId);
      if (params.assistantId) query.append('assistantId', params.assistantId);
      const queryString = query.toString() ? `?${query.toString()}` : '';
      return request(`/MangakaAssistant${queryString}`);
    },
    getById: (id) => request(`/MangakaAssistant/${id}`),
    create: (contractData) => request('/MangakaAssistant', {
      method: 'POST',
      body: JSON.stringify(contractData),
    }),
    update: (id, contractData) => request(`/MangakaAssistant/${id}`, {
      method: 'PUT',
      body: JSON.stringify(contractData),
    }),
    updateStatus: (id, statusDto) => request(`/MangakaAssistant/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify(statusDto),
    }),
    uploadContractFile: (id, file) => {
      const formData = new FormData();
      formData.append('file', file);
      return request(`/MangakaAssistant/${id}/upload-file`, {
        method: 'PUT',
        body: formData,
      });
    },
    delete: (id) => request(`/MangakaAssistant/${id}`, {
      method: 'DELETE',
    }),
  },

  // === BOARD EVALUATIONS ===
  boardEvaluation: {
    getAll: () => request('/BoardEvaluation'),
    getById: (id) => request(`/BoardEvaluation/${id}`),
    create: (evaluationData) => request('/BoardEvaluation', {
      method: 'POST',
      body: JSON.stringify(evaluationData),
    }),
    update: (id, evaluationData) => request(`/BoardEvaluation/${id}`, {
      method: 'PUT',
      body: JSON.stringify(evaluationData),
    }),
    delete: (id) => request(`/BoardEvaluation/${id}`, {
      method: 'DELETE',
    }),
    createBatch: (batchData) => request('/BoardEvaluation/batch', {
      method: 'POST',
      body: JSON.stringify(batchData),
    }),
    getBatchSummary: (evaluationId) => request(`/BoardEvaluation/${evaluationId}/summary`),
  },

  // === SUBMISSIONS ===
  submissions: {
    getEditorialBoardQueue: () => request('/Submissions/eb'),
    getEvaluationsBySeries: (seriesId) => request(`/Submissions/${seriesId}/evaluations`),
    savePartialScore: (seriesId, partialGradeInput) => request(`/Submissions/${seriesId}/score`, {
      method: 'PATCH',
      body: JSON.stringify(partialGradeInput),
    }),
    getEvaluatorsStatus: (seriesId) => request(`/Submissions/${seriesId}/evaluators-status`),
    updateGeneralFeedback: (seriesId, feedbackInput) => request(`/Submissions/${seriesId}/general-feedback`, {
      method: 'PATCH',
      body: JSON.stringify(feedbackInput),
    }),
  },

  // === RANKINGS ===
  rankings: {
    importWeeklyRanking: (formData) => request('/Rankings/import', {
      method: 'POST',
      body: formData, // FormData with ExcelFile and other import metadata
    }),
    getRankingsByIssue: (issueYear, issueNumber) => request(`/Rankings/${issueYear}/${issueNumber}`),
    getRankingHistoryForSeries: (seriesId) => request(`/Rankings/series/${seriesId}`),
  },

  // === NOTIFICATIONS ===
  notifications: {
    getMyNotifications: () => request('/Notifications'),
    getUnreadCount: () => request('/Notifications/unread-count'),
    markAsRead: (id) => request(`/Notifications/${id}/read`, {
      method: 'PATCH',
    }),
    markAllAsRead: () => request('/Notifications/read-all', {
      method: 'PATCH',
    }),
  },

  // === GENRES ===
  genres: {
    getAll: () => request('/Genres'),
    getById: (id) => request(`/Genres/${id}`),
    create: (genreData) => request('/Genres', {
      method: 'POST',
      body: JSON.stringify(genreData),
    }),
    update: (id, genreData) => request(`/Genres/${id}`, {
      method: 'PUT',
      body: JSON.stringify(genreData),
    }),
    delete: (id) => request(`/Genres/${id}`, {
      method: 'DELETE',
    }),
  },

  // === TAGS ===
  tags: {
    getAll: () => request('/Tags'),
    getById: (id) => request(`/Tags/${id}`),
    create: (tagData) => request('/Tags', {
      method: 'POST',
      body: JSON.stringify(tagData),
    }),
    update: (id, tagData) => request(`/Tags/${id}`, {
      method: 'PUT',
      body: JSON.stringify(tagData),
    }),
    delete: (id) => request(`/Tags/${id}`, {
      method: 'DELETE',
    }),
  },

  // === DASHBOARDS ===
  dashboard: {
    getTopSeries: () => request('/Dashboard/TopSeries'),
    getAdminOverview: () => request('/Dashboard/Admin/Overview'),
    getAdminSeriesStats: () => request('/Dashboard/Admin/SeriesStats'),
  },
};
