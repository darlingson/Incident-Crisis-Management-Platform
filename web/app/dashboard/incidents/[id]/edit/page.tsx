'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Save, X, Loader2, Edit3 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

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
            <Card className="max-w-3xl mx-auto">
                <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                        <Edit3 className="w-5 h-5 text-primary" /> Edit incident #{id}
                    </CardTitle>
                    <CardDescription>Update details and owner for this report.</CardDescription>
                </CardHeader>
                <CardContent>
                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* Title */}
                    <div className="space-y-2">
                        <Label htmlFor="title">Title</Label>
                        <Input
                            id="title"
                            required
                            value={formData.title}
                            onChange={e => setFormData({...formData, title: e.target.value})}
                        />
                    </div>

                    {/* Narrative */}
                    <div className="space-y-2">
                        <Label htmlFor="narrative">Narrative / Description</Label>
                        <Textarea
                            id="narrative"
                            rows={4}
                            required
                            value={formData.narrative}
                            onChange={e => setFormData({...formData, narrative: e.target.value, description: e.target.value})}
                        />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Impact Select */}
                        <div className="space-y-2">
                            <Label>Impact Level</Label>
                            <Select value={formData.impact} onValueChange={(v: string) => setFormData({...formData, impact: v})}>
                                <SelectTrigger>
                                    <SelectValue />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="Low">Low</SelectItem>
                                    <SelectItem value="Medium">Medium</SelectItem>
                                    <SelectItem value="High">High</SelectItem>
                                    <SelectItem value="Critical">Critical</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>

                        {/* Assigned To Dropdown */}
                        <div className="space-y-2">
                            <Label>Assignee</Label>
                            <Select value={formData.assignedTo} onValueChange={(v: string) => setFormData({...formData, assignedTo: v})}>
                                <SelectTrigger>
                                    <SelectValue placeholder="Unassigned" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="">Unassigned</SelectItem>
                                    {users.map(user => (
                                        <SelectItem key={user.id} value={user.fullName}>
                                            {user.fullName} ({user.email})
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                    </div>

                    {/* Location & Type */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-2">
                            <Label htmlFor="location">Location</Label>
                            <Input
                                id="location"
                                value={formData.location}
                                onChange={e => setFormData({...formData, location: e.target.value})}
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="type">Incident Type</Label>
                            <Input
                                id="type"
                                value={formData.type}
                                onChange={e => setFormData({...formData, type: e.target.value})}
                            />
                        </div>
                    </div>

                    {/* Form Actions */}
                    <div className="flex gap-4 pt-6">
                        <Button
                            type="submit"
                            disabled={saving}
                            className="flex-1"
                        >
                            {saving ? <Loader2 className="animate-spin w-4 h-4 mr-2" /> : <Save className="w-4 h-4 mr-2" />}
                            {saving ? 'Saving...' : 'Update incident'}
                        </Button>
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => router.back()}
                        >
                            <X className="w-4 h-4 mr-2" /> Cancel
                        </Button>
                    </div>
                </form>
                </CardContent>
            </Card>
        </div>
    );
}