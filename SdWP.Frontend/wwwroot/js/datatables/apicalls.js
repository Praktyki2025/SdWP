window.projectsApi = {
    fetchAllProjects: async function () {
        const response = await fetch('/api/project/all');
        if (!response.ok) throw new Error('Failed to fetch projects');
        return await response.json();
    },
    deleteProject: async function (id) {
        const response = await fetch(`/api/project/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed to delete project');
        return response;
    }
};
