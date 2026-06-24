const TYPE_EMOJI = ['📦', '📱', '👟', '🍎', '🏠', '🎧', '⚡', '🧴', '🧸', '🛋'];

export function emojiForType(type) {
  const str = String(type ?? '');
  let hash = 0;
  for (let i = 0; i < str.length; i++) hash = (hash * 31 + str.charCodeAt(i)) % TYPE_EMOJI.length;
  return TYPE_EMOJI[Math.abs(hash)] || '📦';
}
