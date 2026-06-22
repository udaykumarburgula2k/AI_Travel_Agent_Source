import React, { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { createPlan, getPlans, regeneratePlan } from './api';
import './style.css';

function App() {
  const [form, setForm] = useState({ destination: 'Goa', days: 3, interests: 'Beach, Food', budgetLevel: 'Medium', travelerType: 'Couple' });
  const [plan, setPlan] = useState(null);
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function loadHistory() {
    try { setHistory(await getPlans()); } catch { }
  }

  useEffect(() => { loadHistory(); }, []);

  async function submit(e) {
    e.preventDefault();
    setLoading(true); setError('');
    try {
      const payload = { ...form, days: Number(form.days), interests: form.interests.split(',').map(x => x.trim()).filter(Boolean) };
      const result = await createPlan(payload);
      setPlan(result);
      await loadHistory();
    } catch (err) { setError(err.message); }
    finally { setLoading(false); }
  }

  async function regenerate() {
    if (!plan) return;
    setLoading(true); setError('');
    try {
      const result = await regeneratePlan(plan.planId, { interests: form.interests.split(',').map(x => x.trim()), budgetLevel: form.budgetLevel });
      setPlan(result);
      await loadHistory();
    } catch (err) { setError(err.message); }
    finally { setLoading(false); }
  }

  return <div className="container">
    <h1>AI Travel Itinerary Assistant</h1>
    <form onSubmit={submit} className="card form">
      <label>Destination <input value={form.destination} onChange={e => setForm({ ...form, destination: e.target.value })} /></label>
      <label>Days <input type="number" min="1" max="15" value={form.days} onChange={e => setForm({ ...form, days: e.target.value })} /></label>
      <label>Interests <input value={form.interests} onChange={e => setForm({ ...form, interests: e.target.value })} /></label>
      <label>Budget Level
        <select value={form.budgetLevel} onChange={e => setForm({ ...form, budgetLevel: e.target.value })}>
          <option>Budget</option><option>Medium</option><option>Luxury</option>
        </select>
      </label>
      <label>Traveler Type
        <select value={form.travelerType} onChange={e => setForm({ ...form, travelerType: e.target.value })}>
          <option>Solo</option><option>Couple</option><option>Family</option><option>Group</option>
        </select>
      </label>
      <button disabled={loading}>{loading ? 'Generating...' : 'Generate Itinerary'}</button>
    </form>

    {error && <p className="error">{error}</p>}

    {plan && <section className="card">
      <div className="topline"><h2>{plan.destination}</h2><span>{plan.budgetBand}</span></div>
      <p>{plan.tripSummary}</p>
      {plan.fallbackActivated && <div className="warning">Fallback itinerary used because AI failed or returned invalid output.</div>}
      <button onClick={regenerate} disabled={loading}>Regenerate</button>
      <div className="days">
        {plan.dayWisePlan.map(d => <div className="day" key={d.day}>
          <h3>Day {d.day}</h3>
          <p><b>Morning:</b> {d.morning}</p>
          <p><b>Afternoon:</b> {d.afternoon}</p>
          <p><b>Evening:</b> {d.evening}</p>
          <p><b>Notes:</b> {d.notes}</p>
        </div>)}
      </div>
    </section>}

    <section className="card">
      <h2>Saved Itineraries</h2>
      {history.map(h => <div className="history" key={h.planId} onClick={() => setPlan(h)}>
        {h.destination} - {h.days} days - {h.status}
      </div>)}
    </section>
  </div>;
}

createRoot(document.getElementById('root')).render(<App />);
