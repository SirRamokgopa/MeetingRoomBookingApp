<script setup>
const props = defineProps({
  show: { type: Boolean, required: true },
  title: { type: String, required: true },
  description: { type: String, required: true },
  confirmLabel: { type: String, default: 'Delete' },
  cancelLabel: { type: String, default: 'Cancel' },
  danger: { type: Boolean, default: true }
})
const emit = defineEmits(['confirm', 'cancel'])
</script>

<template>
  <div v-if="show" class="modal-backdrop" role="dialog" aria-modal="true">
    <div class="modal">
      <h3>{{ title }}</h3>
      <p>{{ description }}</p>
      <div class="modal-actions">
        <button :class="{ danger }" @click="$emit('confirm')">{{ confirmLabel }}</button>
        <button @click="$emit('cancel')">{{ cancelLabel }}</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(3, 5, 10, 0.72);
  backdrop-filter: blur(3px);
  padding: 1rem;
  z-index: 50;
}
.modal {
  background: linear-gradient(180deg, var(--c-surface-2), var(--c-surface-1));
  border: 1px solid var(--c-border);
  border-radius: var(--radius-lg);
  padding: 1.5rem;
  max-width: 90%;
  width: 400px;
  box-shadow: var(--shadow-elevated);
}
.modal h3 {
  margin-bottom: 0.5rem;
}
.modal p {
  color: var(--c-text-muted);
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 1.5rem;
}
.modal button.danger {
  border-color: rgba(240, 85, 108, 0.45);
  color: var(--c-danger);
}
.modal button.danger:hover {
  background: var(--c-danger-bg);
  border-color: var(--c-danger);
  color: #fff5f6;
}
</style>
