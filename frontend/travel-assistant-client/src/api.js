const API_BASE = 'https://localhost:64708/api/trips';

export async function createPlan(payload) {
  const res = await fetch(`${API_BASE}/plans`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload)
  });
  if (!res.ok) throw new Error('Failed to create itinerary');
  return res.json();
}

export async function getPlans() {
  const res = await fetch(`${API_BASE}/plans`);
  if (!res.ok) throw new Error('Failed to load itineraries');
  return res.json();
}

export async function regeneratePlan(planId, payload) {
  const res = await fetch(`${API_BASE}/plans/${planId}/regenerate`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload)
  });
  if (!res.ok) throw new Error('Failed to regenerate itinerary');
  return res.json();
}
