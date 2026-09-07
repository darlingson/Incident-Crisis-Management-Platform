'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Save, X, Loader2, Edit3, User } from 'lucide-react';

interface AssignableUser {
    id: string;
    fullName: string;
    email: string;
}

export default function EditReportPage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();
    
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [users, setUsers] = useState<AssignableUser[]>([]);
    
    const [formData, setFormData] = useState({
        title: '',
        narrative: '',
        description: '',
        impact: '',
        location: '',
        assignedTo: '',
        type: ''
    });

    useEffect(() => {
        const loadPageData = async () => {
            try {
                const [reportRes, usersRes] = await Promise.all([
                    fetch(`/api/reports/${id}`),
                    fetch('/api/reports/assignable-users')
                ]);

                const reportData = await reportRes.json();
                const usersData = await usersRes.json();

                setUsers(usersData);
                setFormData({
                    title: reportData.title || '',
                    narrative: reportData.narrative || '',
                    description: reportData.description || reportData.narrative || '',
                    impact: reportData.impact || 'Low',
                    location: reportData.location || '',
                    assignedTo: reportData.assignedTo || '',
                    type: reportData.type || 'General'
                });
            } catch (error) {
                console.error("Failed to load edit data", error);
            } finally {
                setLoading(false);
            }
        };

        if (id) loadPageData();
    }, [id]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        
        try {
            const res = await fetch(`/api/reports/${id}/edit`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData),
            });

            if (res.ok) {
                router.push(`/dashboard/incidents/${id}`);
                router.refresh(); // Ensure details page gets fresh data
            } else {
                const err = await res.json();
                alert(err.error || "Failed to update report");
                setSaving(false);
            }
        } catch (error) {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="bg-background min-h-screen flex items-center justify-center">
                <Loader2 className="animate-spin mr-2" /> Loading...
            </div>
        );
    }

    return (
        <div className="bg-background min-h-screen p-6">
            <div className="max-w-3xl mx-auto bg-card border border-border rounded-lg p-8 shadow-sm">
                <div className="mb-8">
                    <h1 className="text-xl font-semibold flex items-center gap-2">
                        <Edit3 className="w-5 h-5 text-primary" /> Edit incident #{id}
                    </h1>
                    <p className="text-muted-foreground text-sm mt-1">Update details and owner for this report.</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* Title */}
                    <div>
                        <label className="block text-xs font-medium text-muted-foreground mb-2">Title</label>
                        <input 
                            required
                            className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary focus:ring-1 focus:ring-primary outline-none"
                            value={formData.title}
                            onChange={e => setFormData({...formData, title: e.target.value})}
                        />
                    </div>

                    {/* Narrative */}
                    <div>
                        <label className="block text-xs font-medium text-muted-foreground mb-2">Narrative / Description</label>
                        <textarea 
                            rows={4}
                            required
                            className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary focus:ring-1 focus:ring-primary outline-none"
                            value={formData.narrative}
                            onChange={e => setFormData({...formData, narrative: e.target.value, description: e.target.value})}
                        />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Impact Select */}
                        <div>
                            <label className="block text-xs font-medium text-muted-foreground mb-2">Impact Level</label>
                            <select 
                                className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary outline-none"
                                value={formData.impact}
                                onChange={e => setFormData({...formData, impact: e.target.value})}
                            >
                                <option value="Low">Low</option>
                                <option value="Medium">Medium</option>
                                <option value="High">High</option>
                                <option value="Critical">Critical</option>
                            </select>
                        </div>

                        {/* Assigned To Dropdown */}
                        <div>
                            <label className="block text-xs font-medium text-muted-foreground mb-2">Assignee</label>
                            <div className="relative">
                                <select 
                                    className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary outline-none appearance-none"
                                    value={formData.assignedTo}
                                    onChange={e => setFormData({...formData, assignedTo: e.target.value})}
                                >
                                    <option value="">Unassigned</option>
                                    {users.map(user => (
                                        <option key={user.id} value={user.fullName}>
                                            {user.fullName} ({user.email})
                                        </option>
                                    ))}
                                </select>
                                <div className="absolute inset-y-0 right-3 flex items-center pointer-events-none text-muted-foreground">
                                    <User className="w-4 h-4" />
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Location & Type */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-xs font-medium text-muted-foreground mb-2">Location</label>
                            <input 
                                className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary outline-none"
                                value={formData.location}
                                onChange={e => setFormData({...formData, location: e.target.value})}
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-medium text-muted-foreground mb-2">Incident Type</label>
                            <input 
                                className="w-full bg-background border border-border rounded-lg p-3 focus:border-primary outline-none"
                                value={formData.type}
                                onChange={e => setFormData({...formData, type: e.target.value})}
                            />
                        </div>
                    </div>

                    {/* Form Actions */}
                    <div className="flex gap-4 pt-6 border-t border-border">
                        <button 
                            type="submit" 
                            disabled={saving}
                            className="flex-1 bg-primary hover:bg-primary/90 text-primary-foreground font-medium py-3 rounded-lg flex items-center justify-center gap-2 disabled:opacity-50 transition-colors"
                        >
                            {saving ? <Loader2 className="animate-spin w-4 h-4" /> : <Save className="w-4 h-4" />} 
                            {saving ? 'Saving...' : 'Update incident'}
                        </button>
                        <button 
                            type="button"
                            onClick={() => router.back()}
                            className="px-8 bg-background border border-border text-muted-foreground font-medium py-3 rounded-lg flex items-center gap-2 hover:bg-muted transition-colors"
                        >
                            <X className="w-4 h-4" /> Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}