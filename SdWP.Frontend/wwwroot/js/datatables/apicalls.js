window.projectsApi = {
    deleteProject: async function (id) {
        const response = await fetch(`/api/projects/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed to delete project');
        return response;
    }
};
