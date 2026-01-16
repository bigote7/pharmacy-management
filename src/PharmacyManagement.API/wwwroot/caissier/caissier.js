// Utilitaires pour le module Caissier

const API_BASE = '/api/caissier';

// Vérifier l'authentification
function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/login.html';
        return false;
    }
    return true;
}

// Headers avec authentification
function getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
        console.warn('Aucun token trouvé dans localStorage');
    }
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };
}

// Déconnexion
function logout() {
    if (confirm('Voulez-vous vraiment vous déconnecter ?')) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login.html';
    }
}

// Vérifier l'auth au chargement
if (typeof document !== 'undefined') {
    document.addEventListener('DOMContentLoaded', () => {
        if (!checkAuth()) return;
    });
}

